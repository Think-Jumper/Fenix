﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Transport.Libuv.Native
{
    using DotNetty.Common.Internal.Logging;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    sealed unsafe class Loop : IDisposable
    {
        private static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<Loop>();
        private static readonly uv_walk_cb WalkCallback = OnWalkCallback;

        private IntPtr _handle;

        public Loop()
        {
            IntPtr loopHandle = NativeMethods.Allocate(NativeMethods.uv_loop_size().ToInt32());
            try
            {
                int result = NativeMethods.uv_loop_init(loopHandle);
                NativeMethods.ThrowIfError(result);
            }
            catch
            {
                NativeMethods.FreeMemory(loopHandle);
                throw;
            }

            GCHandle gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);
            ((uv_loop_t*)loopHandle)->data = GCHandle.ToIntPtr(gcHandle);
            _handle = loopHandle;
            if (Logger.InfoEnabled)
            {
                Logger.LoopAllocated(_handle);
            }
        }

        internal IntPtr Handle => _handle;

        public bool IsAlive => _handle != IntPtr.Zero && NativeMethods.uv_loop_alive(_handle) != 0;

        public void UpdateTime()
        {
            Validate();
            NativeMethods.uv_update_time(Handle);
        }

        public long Now
        {
            get
            {
                Validate();
                return NativeMethods.uv_now(_handle);
            }
        }

        public long NowInHighResolution
        {
            get
            {
                Validate();
                return NativeMethods.uv_hrtime(_handle);
            }
        }

        public int GetBackendTimeout()
        {
            Validate();
            return NativeMethods.uv_backend_timeout(_handle);
        }

        public int ActiveHandleCount() => 
            _handle != IntPtr.Zero
            ? (int)((uv_loop_t*)_handle)->active_handles 
            : 0;

        public int Run(uv_run_mode mode)
        {
            Validate();
            return NativeMethods.uv_run(_handle, mode);
        }

        public void Stop()
        {
            if (_handle != IntPtr.Zero)
            {
                NativeMethods.uv_stop(_handle);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Validate()
        {
            if (_handle == IntPtr.Zero)
            {
                ThrowObjectDisposedException();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowObjectDisposedException()
        {
            throw GetObjectDisposedException();

            static ObjectDisposedException GetObjectDisposedException()
            {
                return new ObjectDisposedException($"{typeof(Loop)}");
            }
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        void Close()
        {
            IntPtr loopHandle = _handle;
            Close(loopHandle);
            _handle = IntPtr.Zero;
        }

        static void Close(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }

            // Get gc handle before close loop
            IntPtr pHandle = ((uv_loop_t*)handle)->data;

            // Fully close the loop, similar to 
            //https://github.com/libuv/libuv/blob/v1.x/test/task.h#L190

            int count = 0;
            var debugEnabled = Logger.DebugEnabled;
            while (true)
            {
                if (debugEnabled) Logger.LoopWalkingHandles(handle, count);
                NativeMethods.uv_walk(handle, WalkCallback, handle);

                if (debugEnabled) Logger.LoopRunningDefaultToCallCloseCallbacks(handle, count);
                _ = NativeMethods.uv_run(handle, uv_run_mode.UV_RUN_DEFAULT);

                int result = NativeMethods.uv_loop_close(handle);
                if (debugEnabled) Logger.LoopCloseResult(handle, result, count);
                if (0u >= (uint)result)
                {
                    break;
                }

                count++;
                if (count >= 20)
                {
                    Logger.LoopCloseAllHandlesLimit20TimesExceeded(handle);
                    break;
                }
            }
            var infoEnabled = Logger.InfoEnabled;
            if (infoEnabled) Logger.LoopClosed(handle, count);

            // Free GCHandle
            if (pHandle != IntPtr.Zero)
            {
                GCHandle nativeHandle = GCHandle.FromIntPtr(pHandle);
                if (nativeHandle.IsAllocated)
                {
                    nativeHandle.Free();
                    ((uv_loop_t*)handle)->data = IntPtr.Zero;
                    if (infoEnabled) Logger.LoopGCHandleReleased(handle);
                }
            }

            // Release memory
            NativeMethods.FreeMemory(handle);
            if (infoEnabled) Logger.LoopMemoryReleased(handle);
        }

        static void OnWalkCallback(IntPtr handle, IntPtr loopHandle)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }

            try
            {
                // All handles must implement IDisposable
                var target = NativeHandle.GetTarget<IDisposable>(handle);
                target?.Dispose();
                if (Logger.DebugEnabled) Logger.LoopWalkCallbackDisposed(handle, loopHandle, target);
            }
            catch (Exception exception)
            {
                if (Logger.WarnEnabled) Logger.LoopWalkCallbackAttemptToCloseHandleFailed(handle, loopHandle, exception);
            }
        }

        ~Loop() => Close();
    }
}
