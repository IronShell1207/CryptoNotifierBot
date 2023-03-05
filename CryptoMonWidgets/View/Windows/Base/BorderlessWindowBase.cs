﻿using CryptoMonWidgets.Enums.WindowHelper;
using Direct2D;
using DXGI;
using GlobalStructures;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using static DXGI.DXGITools;
using static GDIPlus.GDIPlusTools;

namespace CryptoMonWidgets.View.Windows.Base
{
    public partial class BorderlessWindow : Window
    {
        #region Private Fields

        private Microsoft.UI.Windowing.AppWindow _apw;

        private Microsoft.UI.Windowing.OverlappedPresenter _presenter;

        private bool bMoving = false;

        private bool bSet = false;

        private IntPtr hWndMain = IntPtr.Zero;

        private IntPtr m_hBitmap = IntPtr.Zero;

        private IntPtr m_initToken = IntPtr.Zero;

        private ID2D1DeviceContext m_pD2DDeviceContext = null;

        private ID2D1Factory m_pD2DFactory = null;

        private ID2D1Factory1 m_pD2DFactory1 = null;

        private ID3D11DeviceContext m_pD3D11DeviceContext = null;

        private IntPtr m_pD3D11DevicePtr = IntPtr.Zero;

        private IDXGIDevice1 m_pDXGIDevice = null;

        private IDXGISwapChain1 m_pDXGISwapChain1 = null;

        private int nX = 0, nY = 0, nXWindow = 0, nYWindow = 0;

        #endregion Private Fields

        #region Public Interfaces

        [ComImport, Guid("63aad0b8-7c24-40ff-85a8-640d944cc325"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISwapChainPanelNative
        {
            [PreserveSig]
            HRESULT SetSwapChain(IDXGISwapChain swapChain);
        }

        #endregion Public Interfaces

        #region Public Methods

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("Dwmapi.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern HRESULT DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

        [DllImport("Dwmapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //[DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        //public static extern bool ShouldSystemUseDarkMode();
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out PointInt32 lpPoint);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetObject(IntPtr hFont, int nSize, out BITMAP bm);

        public static long GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32(hWnd, nIndex);
            }
            return GetWindowLongPtr64(hWnd, nIndex);
        }

        [DllImport("User32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern long GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("User32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern long GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        //[DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#132")]
        //public static extern bool ShouldAppsUseDarkMode();
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            }
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, IntPtr pptDst, IntPtr psize, IntPtr hdcSrc, IntPtr pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

        public HRESULT CreateD2D1Factory()
        {
            HRESULT hr = HRESULT.S_OK;
            D2D1_FACTORY_OPTIONS options = new D2D1_FACTORY_OPTIONS();

            // Needs "Enable native code Debugging"
            options.debugLevel = D2D1_DEBUG_LEVEL.D2D1_DEBUG_LEVEL_INFORMATION;

            hr = D2DTools.D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, ref D2DTools.CLSID_D2D1Factory, ref options, out m_pD2DFactory);
            m_pD2DFactory1 = (ID2D1Factory1)m_pD2DFactory;
            return hr;
        }

        public HRESULT CreateDeviceContext()
        {
            HRESULT hr = HRESULT.S_OK;
            uint creationFlags = (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT;

            // Needs "Enable native code Debugging"
            creationFlags |= (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG;

            int[] aD3D_FEATURE_LEVEL = new int[] { (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
                (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_1, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_3,
                (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_2, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1};

            D3D_FEATURE_LEVEL featureLevel;
            hr = D2DTools.D3D11CreateDevice(null,    // specify null to use the default adapter
                D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
                IntPtr.Zero,
                creationFlags,      // optionally set debug and Direct2D compatibility flags
                aD3D_FEATURE_LEVEL, // list of feature levels this app can support
                                    // (uint)Marshal.SizeOf(aD3D_FEATURE_LEVEL),   // number of possible feature levels
                (uint)aD3D_FEATURE_LEVEL.Length, // number of possible feature levels
                D2DTools.D3D11_SDK_VERSION,
                out m_pD3D11DevicePtr,    // returns the Direct3D device created
                out featureLevel,         // returns feature level of device created
                out m_pD3D11DeviceContext // returns the device immediate context
            );
            if (hr == HRESULT.S_OK)
            {
                m_pDXGIDevice = Marshal.GetObjectForIUnknown(m_pD3D11DevicePtr) as IDXGIDevice1;
                if (m_pD2DFactory1 != null)
                {
                    ID2D1Device pD2DDevice = null;
                    hr = m_pD2DFactory1.CreateDevice(m_pDXGIDevice, out pD2DDevice);
                    if (hr == HRESULT.S_OK)
                    {
                        hr = pD2DDevice.CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS.D2D1_DEVICE_CONTEXT_OPTIONS_NONE, out m_pD2DDeviceContext);
                        GlobalTools.SafeRelease(ref pD2DDevice);
                    }
                }
            }
            return hr;
        }

        public void Initialize()
        {
            HRESULT hr = HRESULT.S_OK;

            hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWndMain);
            _apw = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);

            _presenter = _apw.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            _presenter.IsResizable = false;
            _presenter.SetBorderAndTitleBar(false, false);

            _apw.Resize(new SizeInt32(100, 300));
            _apw.Move(new PointInt32(600, 300));

            int nValue = (int)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT;
            hr = DwmSetWindowAttribute(hWndMain, (int)DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref nValue, Marshal.SizeOf(typeof(int)));

            this.Closed += MainWindow_Closed;

            hr = CreateD2D1Factory();
            if (hr == HRESULT.S_OK)
            {
                hr = CreateDeviceContext();
                hr = CreateSwapChain(IntPtr.Zero);
                if (hr == HRESULT.S_OK)
                {
                    ISwapChainPanelNative panelNative = WinRT.CastExtensions.As<ISwapChainPanelNative>(swapChainPanel1);
                    hr = panelNative.SetSwapChain(m_pDXGISwapChain1);
                }
            }

            long nExStyle = GetWindowLong(hWndMain, WindowHelperEnums.GWL_EXSTYLE);
            if ((nExStyle & WindowHelperEnums.WS_EX_LAYERED) == 0)
            {
                SetWindowLong(hWndMain, WindowHelperEnums.GWL_EXSTYLE, (IntPtr)(nExStyle | WindowHelperEnums.WS_EX_LAYERED));

                // Test Light Mode
                int nAppsUseLightTheme = 0;
                int nSystemUsesLightTheme = 0;
                string sPathKey = @"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
                using (RegistryKey rkLocal = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    using (RegistryKey rk = rkLocal.OpenSubKey(sPathKey, false))
                    {
                        nAppsUseLightTheme = (int)rk.GetValue("AppsUseLightTheme", 0);
                        nSystemUsesLightTheme = (int)rk.GetValue("SystemUsesLightTheme", 0);
                    }
                }
                uint nColorBackground = (uint)System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.Black);
                //if (nAppsUseLightTheme == 1 || nSystemUsesLightTheme == 1)
                if (nAppsUseLightTheme == 1)
                {
                    nColorBackground = (uint)System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.White);
                    // not refreshed when mouse over...
                    // myButton.Foreground = new SolidColorBrush(Microsoft.UI.Colors.White);
                }
                bool bReturn = SetLayeredWindowAttributes(hWndMain, nColorBackground, 255, WindowHelperEnums.LWA_COLORKEY);
            }

            UIElement root = (UIElement)this.Content;
            root.PointerMoved += Root_PointerMoved;
            root.PointerPressed += Root_PointerPressed;
            root.PointerReleased += Root_PointerReleased;
        }

        #endregion Public Methods

        #region Private Methods

        private void Clean()
        {
            GlobalTools.SafeRelease(ref m_pD2DDeviceContext);
            GlobalTools.SafeRelease(ref m_pDXGISwapChain1);
            GlobalTools.SafeRelease(ref m_pDXGIDevice);
            GlobalTools.SafeRelease(ref m_pD3D11DeviceContext);
            Marshal.Release(m_pD3D11DevicePtr);
            GlobalTools.SafeRelease(ref m_pD2DFactory1);
            GlobalTools.SafeRelease(ref m_pD2DFactory);
            GdiplusShutdown(m_initToken);
        }

        private HRESULT CreateSwapChain(IntPtr hWnd)
        {
            HRESULT hr = HRESULT.S_OK;
            DXGI_SWAP_CHAIN_DESC1 swapChainDesc = new DXGI_SWAP_CHAIN_DESC1();
            swapChainDesc.Width = 1;
            swapChainDesc.Height = 1;
            swapChainDesc.Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM; // this is the most common swapchain format
            swapChainDesc.Stereo = false;
            swapChainDesc.SampleDesc.Count = 1;                // don't use multi-sampling
            swapChainDesc.SampleDesc.Quality = 0;
            swapChainDesc.BufferUsage = D2DTools.DXGI_USAGE_RENDER_TARGET_OUTPUT;
            swapChainDesc.BufferCount = 2;                     // use double buffering to enable flip
            swapChainDesc.Scaling = (hWnd != IntPtr.Zero) ? DXGI_SCALING.DXGI_SCALING_NONE : DXGI_SCALING.DXGI_SCALING_STRETCH;
            swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL; // all apps must use this SwapEffect
            swapChainDesc.Flags = 0;

            IDXGIAdapter pDXGIAdapter;
            hr = m_pDXGIDevice.GetAdapter(out pDXGIAdapter);
            if (hr == HRESULT.S_OK)
            {
                IntPtr pDXGIFactory2Ptr;
                hr = pDXGIAdapter.GetParent(typeof(IDXGIFactory2).GUID, out pDXGIFactory2Ptr);
                if (hr == HRESULT.S_OK)
                {
                    IDXGIFactory2 pDXGIFactory2 = Marshal.GetObjectForIUnknown(pDXGIFactory2Ptr) as IDXGIFactory2;
                    if (hWnd != IntPtr.Zero)
                        hr = pDXGIFactory2.CreateSwapChainForHwnd(m_pD3D11DevicePtr, hWnd, ref swapChainDesc, IntPtr.Zero, null, out m_pDXGISwapChain1);
                    else
                        hr = pDXGIFactory2.CreateSwapChainForComposition(m_pD3D11DevicePtr, ref swapChainDesc, null, out m_pDXGISwapChain1);

                    hr = m_pDXGIDevice.SetMaximumFrameLatency(1);
                    GlobalTools.SafeRelease(ref pDXGIFactory2);
                    Marshal.Release(pDXGIFactory2Ptr);
                }
                GlobalTools.SafeRelease(ref pDXGIAdapter);
            }
            return hr;
        }

        private void Root_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var properties = e.GetCurrentPoint((UIElement)sender).Properties;
            if (properties.IsLeftButtonPressed)
            {
                PointInt32 pt;
                GetCursorPos(out pt);
                if (bMoving)
                    _apw.Move(new PointInt32(nXWindow + (pt.X - nX), nYWindow + (pt.Y - nY)));
                e.Handled = true;
            }
        }

        private void Root_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var properties = e.GetCurrentPoint((UIElement)sender).Properties;
            if (properties.IsLeftButtonPressed)
            {
                ((UIElement)sender).CapturePointer(e.Pointer);
                nXWindow = _apw.Position.X;
                nYWindow = _apw.Position.Y;
                PointInt32 pt;
                GetCursorPos(out pt);
                nX = pt.X;
                nY = pt.Y;
                bMoving = true;
            }
            else if (properties.IsRightButtonPressed)
            {
                // System.Threading.Thread.Sleep(200);
                // Application.Current.Exit();
            }
        }

        private void Root_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ((UIElement)sender).ReleasePointerCaptures();
            bMoving = false;
        }

        #endregion Private Methods
    }
}