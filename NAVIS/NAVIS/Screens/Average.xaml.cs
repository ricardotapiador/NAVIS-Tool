﻿/////////////////////////////////////////////////////////////////////////////////
//                                                                             //
//    Copyright © 2015  Juan P. Dominguez-Morales                              //
//                                                                             //        
//    This file is part of Neuromorphic Auditory Visualizer Tool (NAVIS Tool). //
//                                                                             //
//    NAVIS Tool is free software: you can redistribute it and/or modify       //
//    it under the terms of the GNU General Public License as published by     //
//    the Free Software Foundation, either version 3 of the License, or        //
//    (at your option) any later version.                                      //
//                                                                             //
//    NAVIS Tool is distributed in the hope that it will be useful,            //
//    but WITHOUT ANY WARRANTY; without even the implied warranty of           //
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the              //
//    GNU General Public License for more details.                             //
//                                                                             //
//    You should have received a copy of the GNU General Public License        //
//    along with NAVIS Tool.  If not, see <http://www.gnu.org/licenses/>.      //
//                                                                             //
/////////////////////////////////////////////////////////////////////////////////


using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NAVIS
{
    /// <summary>
    /// Average activity window.
    /// </summary>
    public partial class Average : Window
    {
        /// <summary>
        /// Average activity empty constructor
        /// </summary>
        public Average()
        {
            InitializeComponent();

            #region chart settings
            chart_Average.Series["Left"].BorderWidth = 2;
            chart_Average.Series["Right"].BorderWidth = 2;

            chart_Average.ChartAreas["ChartArea"].Axes[0].Title = "Timestamp (us)";
            chart_Average.ChartAreas["ChartArea"].Axes[1].Title = "Megaevents fired per second";

            chart_Average.ChartAreas["ChartArea"].Axes[0].TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 11);
            chart_Average.ChartAreas["ChartArea"].Axes[1].TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 11);

            Color[] colores = MainWindow.colorFromSettings();

            chart_Average.Series["Left"].Color = System.Drawing.Color.FromArgb(colores[0].A, colores[0].R, colores[0].G, colores[0].B);
            chart_Average.Series["Right"].Color = System.Drawing.Color.FromArgb(colores[1].A, colores[1].R, colores[1].G, colores[1].B);
            rect_leftCochlea.Fill = new SolidColorBrush(Color.FromArgb(colores[0].A, colores[0].R, colores[0].G, colores[0].B));
            rect_rightCochlea.Fill = new SolidColorBrush(Color.FromArgb(colores[1].A, colores[1].R, colores[1].G, colores[1].B));

            chart_Average.ChartAreas["ChartArea"].AxisX.Minimum = 0;
            chart_Average.ChartAreas["ChartArea"].AxisY.Minimum = 0;

            if (MainWindow.settings.MainS.eventSize == 16)
            {
                chart_Average.ChartAreas["ChartArea"].AxisX.Interval = MainWindow.aedatObject16.maxTimestamp / 10;
            }
            else if (MainWindow.settings.MainS.eventSize == 32)
            {
                chart_Average.ChartAreas["ChartArea"].AxisX.Interval = MainWindow.aedatObject32.maxTimestamp / 10;
            }

            #endregion

            double[] media;

            chart_Average.Series.SuspendUpdates();

            if (MainWindow.settings.MainS.eventSize == 16)  // Calculates the average activity and displays it in the chart
            {
                for (long i = 0; i <= MainWindow.aedatObject16.maxTimestamp; i += (int)MainWindow.settings.ToolsS.integrationPeriod)
                {
                    media = MainWindow.aedatObject16.averageBetweenTimestamps(i, i + MainWindow.settings.ToolsS.integrationPeriod);
                    if (MainWindow.cochleaInfo == EnumCochleaInfo.STEREO32 || MainWindow.cochleaInfo == EnumCochleaInfo.STEREO64)
                    {
                        chart_Average.Series["Right"].Points.AddXY(i, media[1]);
                        chart_Average.Series["Left"].Points.AddXY(i, media[0]);
                    }
                    else if (MainWindow.cochleaInfo == EnumCochleaInfo.MONO32 || MainWindow.cochleaInfo == EnumCochleaInfo.MONO64)
                    {
                        chart_Average.Series["Left"].Points.AddXY(i, media[0]);
                    }
                }
            }
            else if (MainWindow.settings.MainS.eventSize == 32)
            {
                for (long i = 0; i <= MainWindow.aedatObject32.maxTimestamp; i += (int)MainWindow.settings.ToolsS.integrationPeriod)
                {
                    media = MainWindow.aedatObject32.averageBetweenTimestamps(i, i + MainWindow.settings.ToolsS.integrationPeriod);
                    if (MainWindow.cochleaInfo == EnumCochleaInfo.STEREO32 || MainWindow.cochleaInfo == EnumCochleaInfo.STEREO64)
                    {
                        chart_Average.Series["Right"].Points.AddXY(i, media[1]);
                        chart_Average.Series["Left"].Points.AddXY(i, media[0]);
                    }
                    else if (MainWindow.cochleaInfo == EnumCochleaInfo.MONO32 || MainWindow.cochleaInfo == EnumCochleaInfo.MONO64)
                    {
                        chart_Average.Series["Left"].Points.AddXY(i, media[0]);
                    }
                }
            }
            chart_Average.Series.ResumeUpdates();
        }

        /// <summary>
        /// Saves an image with the diagram that is being displayed in the average activity chart
        /// </summary>
        private void B_saveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.Title = "Select a name and a path for the .png file";
            saveImageDialog.Filter = "png files|*.png";
            if (saveImageDialog.ShowDialog() == true)
            {
                chart_Average.SaveImage(saveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);

                InfoWindow iw = new InfoWindow("Success!", "Image saved successfuly");
                iw.ShowDialog();
            }
        }
    }
}