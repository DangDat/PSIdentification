
/*
    Copyright (c) Microsoft Corporation All rights reserved.  
 
    MIT License: 
 
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
    documentation files (the  "Software"), to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
    and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
 
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
 
    THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using PersonIdentification.Pages;
using Microsoft.Band;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using PersonIdentification.Common;
using Microsoft.Band.Sensors;
using System.Threading;
using Windows.UI.Core;
using System.Collections.Generic;
using Windows.Storage;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PersonIdentification.Pages
{
    public partial class BasicsPage
    {
        public string[,] twoD;
        public string[,] twoD1;
        public string[,] twoD3;
        public string[,] twoD4;
        public string change_value ="";
        private BasicsModel model;
        private List<string> TempList = new List<string>();
        private List<string> HeartList = new List<string>();
        private List<string> FinalList = new List<string>();
        private String name_set;
        private double delay_time = 5;
        string myTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss:ffff");
        public BasicsPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.model = new BasicsModel();
            this.DataContext = this.model;

            App.Current.Resuming += App_Resuming;

            var t = this.FindDevicesAsync();
        }

        private async void ConnectDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (model.Main.BandClient == null)
            {
                using (new DisposableAction(() => model.Connecting = true, () => model.Connecting = false))
                {
                    try
                    {
                        // This method will throw an exception upon failure for a veriety of reasons,
                        // such as Band out of range or turned off.
                        model.Main.BandClient = await BandClientManager.Instance.ConnectAsync(model.SelectedDevice);
                    }
                    catch (Exception ex)
                    {
                        var t = new MessageDialog(ex.Message, "Failed to Connect").ShowAsync();
                    }
                }
            }
            else
            {
                model.Main.DisconnectDevice();
            }
        }

        private async void Button_Click_Record(object sender, RoutedEventArgs e)
        {
            ////((Frame)Window.Current.Content).Navigate(typeof(Pages.Drawing), new PassedData { Name_Patient = "Data_Test", Disease_Status = " " });
            this.txt_result4.Text = "";

            //subscribe to skin temperature
            model.Main.BandClient.SensorManager.SkinTemperature.ReadingChanged += SkinTemperature_ReadingChanged;
            model.Main.BandClient.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;

            await model.Main.BandClient.SensorManager.SkinTemperature.StartReadingsAsync(new CancellationToken());
            //subscribe to HR
            
            await model.Main.BandClient.SensorManager.HeartRate.StartReadingsAsync(new CancellationToken());

            // Receive Accelerometer data for a while.
            await Task.Delay(TimeSpan.FromMinutes(delay_time));
            await model.Main.BandClient.SensorManager.SkinTemperature.StopReadingsAsync(new CancellationToken());
            await model.Main.BandClient.SensorManager.HeartRate.StopReadingsAsync(new CancellationToken());
            ProjectFile(TempList, "SkinTemp.txt");
            ProjectFile(HeartList, "HeartRate.txt");
        }


        private async void SkinTemperature_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandSkinTemperatureReading> e)
        {
            //App.Current.InvokeOnUIThread(
            //    () =>
            //    {
            //        model.SkinTemp = e.SensorReading.Temperature;
            //        model.SkinTempTimestamp = e.SensorReading.Timestamp;
            //    }
            //);
            double tem = e.SensorReading.Temperature;
            string text = string.Format("{0} {1} \n", tem,  DateTime.Now.ToString("mmss"));
            TempList.Add(text);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { this.txt_result.Text = text; }).AsTask();
        }

        private async void HeartRate_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandHeartRateReading> e)
        {
            //App.Current.InvokeOnUIThread(
            //    () =>
            //    {
            //        model.HeartRate = e.SensorReading.HeartRate;
            //        model.HRQuality = e.SensorReading.Quality;
            //        model.HrTimestamp = e.SensorReading.Timestamp;
            //    }
            //);
           
            double heart = e.SensorReading.HeartRate;
            string text1 = string.Format("{0} {1} \n", heart, DateTime.Now.ToString("mmss"));
            HeartList.Add(text1);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { this.txt_result1.Text = text1; }).AsTask();
        }

        private async void Button_Click_Identification(object sender, RoutedEventArgs e)
        {
            ////this.txt_result.Text = "Parkinson Possitive: 89%";
             await ReadFile();

        }

        private async Task ReadFile()
        {
            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (local != null)
            {
                // Get the DataFolder folder.
                var dataFolder = await local.GetFolderAsync("Data");

                // Get the file.
                var file = await dataFolder.OpenStreamForReadAsync("HeartRate.txt");
                var file1 = await dataFolder.OpenStreamForReadAsync("SkinTemp.txt");
                string content = null;
                string content1 = null;

                // Read the data.
                using (StreamReader streamReader1 = new StreamReader(file1))
                {
                    content1 = streamReader1.ReadToEnd();
                    //this.TxtPrice.Text = content;
                    var res1 = content1.Split('\n').Select(p => Regex.Split(p, "(?<=\\ )")).ToArray();

                    twoD1 = new String[res1.Length - 1, res1[0].Length - 1];
                    for (int i = 0; i < res1.Length - 1; i++)
                        for (int j = 0; j < res1[0].Length - 1; j++)
                        {
                            twoD1[i, j] = res1[i][j];
                            //twoD[i, res[0].Length - 2] = "";
                            //this.txt_result2.Text += twoD1[i, j].ToString();
                        }
                }
                // Read the data.
                using (StreamReader streamReader = new StreamReader(file))
                {
                    content = streamReader.ReadToEnd();
                    //this.TxtPrice.Text = content;
                    var res = content.Split('\n').Select(p => Regex.Split(p, "(?<=\\ )")).ToArray();

                    twoD = new String[res.Length - 1, res[0].Length - 1];  // Heart rate
                    for (int i = 0; i < res.Length - 1; i++)
                        for (int j = 0; j < res[0].Length - 1; j++)
                        {
                            twoD[i, j] = res[i][j];
                            //twoD[i, res[0].Length - 2] = "";
                            //this.txt_result3.Text += twoD[i, j].ToString();
                        }
                }

                twoD3 = new String[twoD.GetLength(0), twoD.GetLength(1)];
                twoD4 = new String[twoD3.GetLength(0), twoD3.GetLength(1) + 1];
                int k = 0;
                change_value = twoD1[0, 0];
                if (twoD[0, 1] != twoD1[0, 1])
                {
                    change_value = twoD1[0, 0];
                    twoD3[0, 0] = twoD[0, 0];
                    twoD3[0, 1] = twoD1[0, 0];
                    k = 1;
                }

                for (int i = 0; i < twoD3.GetLength(0); i++)
                {           
                    if (twoD[i, 1] == twoD1[k, 1])
                    {
                        change_value = twoD1[k, 0];
                        twoD3[i, 0] = twoD[i, 0];
                        twoD3[i, 1] = twoD1[k, 0];
                        if (k < twoD1.GetLength(0) -1)
                        {
                            k += 1;
                        }
                    }
                    else
                    {
                        twoD3[i, 0] = twoD[i, 0];
                        twoD3[i, 1] = change_value;
                    }
                }

                for (int i = 0; i < twoD3.GetLength(0); i++)
                {
                    string text3 = string.Format("{0} {1} \n", twoD3[i, 0], twoD3[i, 1]);
                    twoD4[i, 0] = twoD3[i, 0];
                    twoD4[i, 1] = twoD3[i, 1];
                    twoD4[i, 2] = "";
                    FinalList.Add(text3);
                }
            }
            ProjectFile(FinalList, "FinalText.txt");

            try
            {
                string result = await AutoPredictionMLAzure.InvokeRequestResponseService(twoD4);
                JObject obj = JObject.Parse(result);
                string[][] values = JsonConvert.DeserializeObject<string[][]>(obj["Results"]["output1"]["value"]["Values"].ToString());

                //TxtPrice.Text = "";
                double total_nega = 0;
                double total_posi = 0;
                double sum_nega = 0;
                double sum_posi = 0;
                double total_result = 0;

                //foreach (string[] item in values)
                //{
                //    if (Convert.ToDouble(item[4]) == -1)
                //    {
                //        total_nega += 1;
                //        total_result += 1;
                //        sum_nega += Convert.ToDouble(item[5]);
                //    }

                //    else if ((Convert.ToDouble(item[4]) == 1))
                //    {
                //        total_posi += 1;
                //        total_result += 1;
                //        sum_posi += Convert.ToDouble(item[5]);
                //    }

                //    //TxtPrice.Text += item[4] + " : " + item[5] + Environment.NewLine;

                //}
                foreach (string[] item in values)
                {
                    if (Convert.ToDouble(item[3]) == -1)
                    {
                        total_nega += 1;
                        total_result += 1;
                        double accuracy = Convert.ToDouble(item[4]);
                        sum_nega += accuracy;
                        sum_posi += (1 - (accuracy));
                    }

                    else if ((Convert.ToDouble(item[3]) == 1))
                    {
                        total_posi += 1;
                        total_result += 1;
                        double accuracy = Convert.ToDouble(item[4]);
                        sum_posi += accuracy;
                        sum_nega += (1 - accuracy);
                    }

                    //TxtPrice.Text += item[4] + " : " + item[5] + Environment.NewLine;

                }


                //txt_result.Text = "Negative Num:" + total_nega.ToString() + "Positive Num:" + total_posi.ToString() + "\n" + "accuracy nega:" + (sum_nega / total_result) + "accuracy posi:" + (sum_posi / total_result) + total_result.ToString();
                //int total_result = total_posi + total_nega;
               // txt_result4.Text = "It is not you:" + ((total_nega / total_result)).ToString("0.0%") + "\n" + "It is you:" + ((total_posi / total_result)).ToString("0.0%") + "\n";
                txt_result4.Text = "It is not you:" + ((sum_nega / total_result)).ToString("0.0%") + "\n" + "It is you:" + ((sum_posi / total_result)).ToString("0.0%") + "\n";
                //"accuracy nega:" + (sum_nega / total_nega) + "accuracy posi:" + (sum_posi / total_posi);

            }
            catch (Exception ex)
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(ex.Message);
                md.ShowAsync();
            }
            //  this.TxtPrice.Text = streamReader.ReadToEnd();
        }
        private async Task FindDevicesAsync()
        {
            IBandInfo selected = model.SelectedDevice;                        

            IBandInfo[] bands = await BandClientManager.Instance.GetBandsAsync();

            model.Devices = new ObservableCollection<IBandInfo>(bands);

            if (selected != null)
            {
                model.SelectedDevice =model.Devices.SingleOrDefault((i) => { return (i.Name == selected.Name); } );
            }
            else if (model.Devices.Count > 0)
            {
                model.SelectedDevice = model.Devices[0];
            }
        }

        private async void ProjectFile(List<string> list, String nameFileData)
        {
            var localAppFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            // Create a new folder name DataFolder.
            var dataFolder = await localAppFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);

            var fileHandle = await dataFolder.CreateFileAsync(nameFileData, Windows.Storage.CreationCollisionOption.ReplaceExisting);

            foreach (string text_file in list)
            {
                await Windows.Storage.FileIO.AppendTextAsync(fileHandle, text_file);
            }
        }
        async void App_Resuming(object sender, object e)
        {
            await this.FindDevicesAsync();
        }
    }
}
