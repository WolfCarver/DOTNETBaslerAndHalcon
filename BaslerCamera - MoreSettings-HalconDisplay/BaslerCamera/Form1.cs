using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basler.Pylon; //引入相机操作软件包
using HalconDotNet; //引入Halcon

using System.Runtime.InteropServices;

namespace BaslerCamera
{
    public partial class Form1 : Form
    {
        //根据相机SN号创建相机
        Camera camera = new Camera("22796063");

        public Form1()
        {
            InitializeComponent();
            //根据SN号创建相机根据创建相机
            camera = new Camera("22796063");

            //开启相机
            camera.Open();
            //加载 User Set 1 user 默认设置
            camera.Parameters[PLCamera.UserSetSelector].SetValue(PLCamera.UserSetSelector.UserSet1);
            camera.Parameters[PLCamera.UserSetLoad].Execute();
            //设置相片宽度、长度
            camera.Parameters[PLCamera.Width].SetValue(1280);
            camera.Parameters[PLCamera.Height].SetValue(960);
            //保存用户设置：此处与官方描述不符，无法将设置保存
            //camera.Parameters[PLCamera.UserSetSave].SetValue(PLCamera.UserSetSelector.UserSet1);
            camera.Parameters[PLCamera.UserSetSave].Execute();
            //将用户设置写入默认设置
            camera.Parameters[PLCamera.UserSetDefault].SetValue(PLCamera.UserSetDefault.UserSet1);
            //设置图片缓存区大小
            camera.Parameters[PLCameraInstance.MaxNumBuffer].SetValue(1);
            //设置相机触发模式为软件触发
            camera.CameraOpened += Configuration.SoftwareTrigger;
            //关闭相机自动曝光
            camera.Parameters[PLCamera.ExposureAuto].SetValue(PLCamera.ExposureAuto.Off);
            //设置相机曝光时间
            camera.Parameters[PLCamera.ExposureTime].SetValue(5000);
        }

        private void MakePicture_Click(object sender, EventArgs e)
        {
            //设置相机开始采集
            camera.StreamGrabber.Start();
            //软件触发相机采集
            camera.ExecuteSoftwareTrigger();
            //获取相机采集结果
            IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);
            //显示相机采集结果
            //ImageWindow.DisplayImage(0, grabResult);
            //采集完成判断相机状态，如果仍在采集则关闭相机采集
            if (camera.StreamGrabber.IsGrabbing)
            {
                camera.StreamGrabber.Stop();
            }

            //锁定像素数据
            GCHandle hand = GCHandle.Alloc(grabResult.PixelData, GCHandleType.Pinned);
            //获取像素数据的指针
            IntPtr imagePointer = hand.AddrOfPinnedObject();
            //转成HOjbect
            HalconDotNet.HObject imageHobject;
            HalconDotNet.HOperatorSet.GenImage1(out imageHobject, new HalconDotNet.HTuple("byte"), 
                grabResult.Width, grabResult.Height, imagePointer);
            //释放内存
            if (hand.IsAllocated) hand.Free();
            //记录图片显示的宽度、高度
            HTuple width, heigth;
            //清屏
            HOperatorSet.ClearWindow(hWindowControl1.HalconWindow);
            //获取图片长度、宽度
            HOperatorSet.GetImageSize(imageHobject, out width, out heigth);
            //设置显示区域
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, heigth, width);
            //显示图片
            HOperatorSet.DispObj(imageHobject, hWindowControl1.HalconWindow);
        }
    }
}
