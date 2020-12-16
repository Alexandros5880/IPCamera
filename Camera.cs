﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

//using VisioForge.Controls.UI.WinForms;
using VisioForge.Controls.UI.WPF;
using VisioForge.Types;
using VisioForge.Types.OutputFormat;
// https://help.visioforge.com/sdks_net/html/T_VisioForge_Controls_UI_WPF_VideoCapture.htm
using VisioForge.Types.VideoEffects;

namespace IPCamera
{
    public class Camera
    {
        public String url = "";
        public string name = "";
        public string id = "";
        public bool detection = false;
        public bool recognition = false;
        public int brightness = 0;
        public int contrast = 0;
        public int darkness = 0;
        public bool recording = false;
        public VideoCapture video;
        public static int count = 0;
        public static String DB_connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Alexp\\source\\repos\\IPCamera\\Database1.mdf;Integrated Security=True";
        public static String pictures_dir;
        public static String videos_dir;
        public static bool avi_format = false;
        public static bool mp4_format = false;
        public static bool webm_format = false;

        public Camera(String url, String name, String id, bool rec)
        {
            this.url = url;
            this.name = name;
            this.id = id;

            // Create an VideoCapture
            this.video = new VideoCapture();
            this.video.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings() { URL = this.url, Type = VisioForge.Types.VFIPSource.RTSP_HTTP_FFMPEG };
            this.video.Audio_PlayAudio = this.video.Audio_RecordAudio = false;
            this.video.Video_Effects_Enabled = true; // Enable Video Effects
            this.Recording = rec;

            count++;
        }

        ~Camera()
        {
            count--;
        }

        public int Brightness
        {
            get { return this.brightness; }
            set
            {
                this.brightness = value;
                // Add the efect
                IVFVideoEffectLightness lightness;
                var effect_l = this.video.Video_Effects_Get("Lightness");
                if (effect_l == null)
                {
                    lightness = new VFVideoEffectLightness(true, this.brightness, 0, "Lightness");
                    this.video.Video_Effects_Add(lightness);
                }
                else
                {
                    lightness = effect_l as IVFVideoEffectLightness;
                    if (lightness != null)
                    {
                        lightness.Value = this.brightness;
                    }
                }
            }
        }

        public int Contrast
        {
            get { return this.contrast; }
            set
            {
                this.contrast = value;
                // Add the efect
                IVFVideoEffectContrast contrast;
                var effect_c = this.video.Video_Effects_Get("Contrast");
                if (effect_c == null)
                {
                    contrast = new VFVideoEffectContrast(true, this.contrast, 0, "Contrast");
                    this.video.Video_Effects_Add(contrast);
                }
                else
                {
                    contrast = effect_c as IVFVideoEffectContrast;
                    if (contrast != null)
                    {
                        contrast.Value = this.contrast;
                    }
                }
            }
        }

        public int Darkness
        {
            get { return this.darkness; }
            set
            {
                this.darkness = value;
                // Add the efect
                IVFVideoEffectDarkness darkness;
                var effect_d = this.video.Video_Effects_Get("Darkness");
                if (effect_d == null)
                {
                    darkness = new VFVideoEffectDarkness(true, this.darkness, 0, "Darkness");
                    this.video.Video_Effects_Add(darkness);
                }
                else
                {
                    darkness = effect_d as IVFVideoEffectDarkness;
                    if (darkness != null)
                    {
                        darkness.Value = this.darkness;
                    }
                }
            }
        }

        public bool Detection
        {
            get
            { return this.detection; }
            set
            {
                this.detection = value;
            }
        }

        public bool Recognition
        {
            get { return this.recognition; }
            set
            {
                this.recognition = value;
            }
        }

        // Start / Stop Recording
        public bool Recording
        {
            get { return this.recording; }
            set
            {
                this.recording = value;
                if (value == true) // Recording
                {
                    DateTime now = DateTime.Now;
                    String date = now.ToString("F");
                    date = date.Replace(":", ".");
                    String dir_path = Camera.videos_dir + "\\" + this.name;
                    if (!Directory.Exists(dir_path))
                    {
                        Directory.CreateDirectory(dir_path);
                    }

                    // Start Recording
                    this.video.Video_FrameRate = 25;
                    this.video.Mode = VFVideoCaptureMode.IPCapture;

                    // AVI
                    if (avi_format)
                    {
                        String file = dir_path + "\\" + date + ".avi";
                        this.video.Output_Filename = file;
                        VFAVIOutput aviout = new VFAVIOutput();
                        this.video.Output_Format = aviout;
                    }

                    // MP4
                    if (mp4_format)
                    {
                        String file = dir_path + "\\" + date + ".mp4";
                        this.video.Output_Filename = file;
                        //this.video.Output_Format = new VFMP4v8v10Output();
                        VFMP4v8v10Output mp4Output = new VFMP4v8v10Output();
                        mp4Output.MP4Mode = VFMP4Mode.v8;
                        this.video.Output_Format = mp4Output;
                    }

                    // WEBM
                    if (webm_format)
                    {
                        String file = dir_path + "\\" + date + ".webm";
                        System.Windows.MessageBox.Show(file);
                        this.video.Output_Filename = file;
                        VFWebMOutput webmout = new VFWebMOutput();
                        this.video.Output_Format = webmout;
                    }


                    
                    
                }
                else // No Recording
                {
                    this.video.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
                }
            }
        }


        public Action<object, MouseButtonEventArgs> MouseUp { get; internal set; }

        public void start()
        {
            try
            {
                this.video.Start();
                //this.video.StartAsync();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("No cameras has found!");
            }
        }

        public void stop()
        {
            try
            {
                //this.video.Stop();
                this.video.StopAsync();
            }
            catch (Exception)
            {
                //System.Windows.MessageBox.Show("No cameras has found!");
            }
        }

        public void take_pic()
        {
            DateTime now = DateTime.Now;
            String date = now.ToString("F");
            date = date.Replace(":", ".");
            String dir_path = Camera.pictures_dir + "\\" + this.name;
            if (! Directory.Exists(dir_path))
            {
                Directory.CreateDirectory(dir_path);
            }
            String file = dir_path + "\\" + date + ".jpg";
            //System.Windows.MessageBox.Show($"Save Picture  {file}");
            this.video.Frame_Save(file, VisioForge.Types.VFImageFormat.JPEG, 85);
        }

    }
}