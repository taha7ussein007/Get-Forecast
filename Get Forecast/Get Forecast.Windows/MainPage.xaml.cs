using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.ApplicationModel.Store;
using Windows.ApplicationModel;
using System.Text.RegularExpressions;




// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Get_Forecast
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        ApplicationDataContainer _roamingNumberOfVisit = ApplicationData.Current.RoamingSettings;
        ApplicationDataContainer _localNumberOfVisit = ApplicationData.Current.LocalSettings;
        ApplicationDataContainer _roamingData = ApplicationData.Current.RoamingSettings;
        ApplicationDataContainer _localData = ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();
        }



        public static bool CheckInternetConnectivity()
        {
            var internetProfile = NetworkInformation.GetInternetConnectionProfile();
            if (internetProfile == null)
                return false;

            return (internetProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }
        private bool ReadAppRatingSetting()
        {
            if (_localData.Values["AppRatingDone"] != null)
                return (bool)_localData.Values["AppRatingDone"];

            if (_roamingData.Values["AppRatingDone"] != null)
                return (bool)_roamingData.Values["AppRatingDone"];

            return false;
        }
        private async void OpenStoreRating(IUICommand command)
        {
            string name = Package.Current.Id.FamilyName;
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + name));
        }

        private async void Rate_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInternetConnectivity())
                return;

            if (ReadAppRatingSetting())
                return;

            MessageDialog msg;
            bool retry = false;
            msg = new MessageDialog("Would you like to rate us now?");
            msg.Commands.Add(new UICommand("yes :)", new UICommandInvokedHandler(this.OpenStoreRating)));
            msg.Commands.Add(new UICommand("Not now", (uiCommand) => { }));
            msg.DefaultCommandIndex = 0;
            msg.CancelCommandIndex = 1;
            await msg.ShowAsync();

            while (retry)
            {
                try { await msg.ShowAsync(); }
                catch { }
            }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));

        }




        //-------------------------------------------------------------------
        int check_method = 1;

        private void choice1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            sp2.Visibility = Visibility.Collapsed;
            sp3.Visibility = Visibility.Collapsed;
            sp4.Visibility = Visibility.Collapsed;
            sp1.Visibility = Visibility.Visible;
            check_method = 1;
        }

        private void choice2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            sp2.Visibility = Visibility.Visible;
            sp3.Visibility = Visibility.Collapsed;
            sp4.Visibility = Visibility.Visible;
            sp1.Visibility = Visibility.Collapsed;
            check_method = 2;
        }

        private void choice3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            sp2.Visibility = Visibility.Collapsed;
            sp3.Visibility = Visibility.Visible;
            sp4.Visibility = Visibility.Collapsed;
            sp1.Visibility = Visibility.Collapsed;
            check_method = 3;
        }

      

        /// ////////////////////////////////////////

        List<double> quans = new List<double>();
        List<int> whiFor = new List<int>();
        int x = 0;
        bool check = false;
        float foreConstant ;
        List<double> Forecast = new List<double>();



        private async void Button_Click(object sender, RoutedEventArgs e)    // add but 
        {

            if (check_method == 1)
            {
                if (numOfPer.Text != "" && Regex.IsMatch(numOfPer.Text, "^[0-9]*$"))
                {
                    sp1.Visibility = Visibility.Collapsed;
                    wh.Visibility = Visibility.Visible;
                }

                else
                {
                    var dialog = new MessageDialog("sorry You Didn't enter some thing wrong about Number Of Periods !");
                    await dialog.ShowAsync();
                }

                if (quan.Text != "" && Regex.IsMatch(quan.Text, "^\\£?[+-]?[\\d,]*\\.?\\d{0,2}$"))
                {
                    quans.Add(Convert.ToDouble(quan.Text));
                }
                else
                {
                    var dialog = new MessageDialog("sorry You Didn't enter any Quantity !");
                    await dialog.ShowAsync();
                }



                if (check == true)
                {
                    wh.Visibility = Visibility.Collapsed;
                }




                if (which.Text != "" && wh.Visibility == Visibility.Visible &&  Regex.IsMatch(which.Text, "^[0-9]*$" )  )
                {
                    whiFor.Add(Convert.ToInt32(which.Text));
                    if (x == 0)
                    {
                        var dialog = new MessageDialog("You Will Get Forecast For Entry Number [ " + which.Text + " ] .");
                        await dialog.ShowAsync();
                        wh.Visibility = Visibility.Collapsed;
                    }
                    wh.Visibility = Visibility.Collapsed;
                    x = 1;
                    check = true;
                }
                else if (which.Text == "" && wh.Visibility == Visibility.Visible && x == 0 )
                {
                    var dialog = new MessageDialog("Notice You didn't enter Which Index to Get Forecast Until Now .");
                    await dialog.ShowAsync();
                    which.Text = string.Empty;
                }




                quan.Text = string.Empty;
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (check_method == 2)
            {
             
               
                if (quan.Text != "" && Regex.IsMatch(quan.Text, "^\\£?[+-]?[\\d,]*\\.?\\d{0,2}$"))
                {
                    quans.Add(Convert.ToDouble(quan.Text));
                    quan.Text = string.Empty;
                }

                else
                {
                    var dialog3 = new MessageDialog("Sorry You Didn't enter right Quantity !");
                    await dialog3.ShowAsync();
                    quan.Text = string.Empty;
                }



                if (firFore.Text != "" && cons.Text != "" && quans.Count > 0 && foreConstant >= 0 && foreConstant <= 1.00 && Regex.IsMatch(cons.Text, "^\\£?[+-]?[\\d,]*\\.?\\d{0,2}$") && Regex.IsMatch(firFore.Text, "^\\£?[+-]?[\\d,]*\\.?\\d{0,2}$"))
                {
                    if(Forecast.Count == 0)
                    Forecast.Add(Convert.ToDouble(firFore.Text)); 
                    foreConstant =float.Parse(cons.Text);
                    sp2.Visibility = Visibility.Collapsed;
                    sp4.Visibility = Visibility.Collapsed;
                    wh.Visibility = Visibility.Visible;
                }

                else
                {
                    var dialog2 = new MessageDialog("Oops There is Some Thing Wrong   \n * Notice : Forecast Constant Must be less than or Equal 1.00 \n\t\t--<| Please Start new Calculation |>--");
                    await dialog2.ShowAsync();
                    firFore.Text = string.Empty;
                    cons.Text = string.Empty;
                }



                if (which.Text != "" && wh.Visibility == Visibility.Visible && Regex.IsMatch(which.Text, "^[0-9]*$") && foreConstant >= 0 && foreConstant <= 1.00  )
                {
                    whiFor.Add(Convert.ToInt32(which.Text));
                    if (x == 0)
                    {
                        var dialog = new MessageDialog("You Will Get Forecast For Entry Number [ " + which.Text + " ] .");
                        await dialog.ShowAsync();
                        wh.Visibility = Visibility.Collapsed;
                    }
                    wh.Visibility = Visibility.Collapsed;
                    x = 1;
                    check = true;
                }
            }



            //////////////////////////////////////////////////////////////////////////////////////////////
            if (check_method == 3)
            {
                var dialog = new MessageDialog("add method 3");
                await dialog.ShowAsync();
            }
        }


        //#####################################################################################
        //#####################################################################################
        //#####################################################################################

        private async void Button_Click_1(object sender, RoutedEventArgs e)    //  cal but
        {

            if (check_method == 1)
            {
                double cal = 0;
                int i, y;

                if ((quans.Count()) != 0 &&  (Convert.ToInt32(quans.Count)) >= (Convert.ToInt32(numOfPer.Text)) && (Convert.ToInt32(which.Text)) >= (Convert.ToInt32(numOfPer.Text)))
                {
                    for (i = 1, y = (Convert.ToInt32(which.Text)); i <= (Convert.ToDouble(numOfPer.Text)); y--, i++)
                    {
                     
                            cal = cal + (quans[y - 2]);
                        
                    }

                    double res = (double)cal / (Convert.ToInt32(numOfPer.Text));
                    var dialog = new Windows.UI.Popups.MessageDialog("Result is -->> | " + res.ToString() + " |");
                    await dialog.ShowAsync();
                }

                else
                {
                    var dialog = new MessageDialog("sorry You Missed Entries !! \n OR \n You didn't Enter Enough Adds to Get This Index 's Forecast !! \n Start new Calculation ");
                    await dialog.ShowAsync();

                }

            }


            //////////////////////////////////////////////////////////////////////////////////////////
            if (check_method == 2)
            {
                double fore = 0;
                int i, y;

                if ((quans.Count()) != 0 && ((Convert.ToInt32(quans.Count)) >= Convert.ToInt32(which.Text)-1))
                {
                    for (i = 1, y = (Convert.ToInt32(which.Text)); i <= (Convert.ToInt32(quans.Count)); y--, i++)  //(Convert.ToInt32(quans.Count))
                    {
                        fore = foreConstant * quans[i - 1] + (1 - foreConstant) * Forecast[i - 1];
                        Forecast.Add(fore);
                    }

                        //double res1 = quans[l];//Convert.ToInt32(which.Text) 
                        //var dialogg = new Windows.UI.Popups.MessageDialog("quan is -->> | " + res1.ToString() + " | \n " + "constant = " + foreConstant + "first forecast = " + firFore);
                        //await dialogg.ShowAsync();
                        ////---------------
                    //for (int p = 0; p < Forecast.Count(); p++)
                    //{
                    double res = Forecast[Convert.ToInt32(which.Text) -1 ];   //Convert.ToInt32(which.Text)
                        var dialog = new Windows.UI.Popups.MessageDialog(" ـــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ \n " + "Your Forecast is : | " + res.ToString() + " |" + "\n ـــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                        await dialog.ShowAsync();
                    
                }

                else
                {
                    var dialog = new MessageDialog("sorry You Missed Entries !! \n OR \n You didn't Enter Enough Adds to Get This Index 's Forecast !! \n Start new Calculation ");
                    await dialog.ShowAsync();

                }
            }


            ///////////////////////////////////////////////////////////////////////////////////////////
            if (check_method == 3)
            {
                var dialog = new MessageDialog("calculate method 3");
                await dialog.ShowAsync();
            }

        }




        //######################################################################
        //######################################################################
        //######################################################################


        private async void Button_Click_2(object sender, RoutedEventArgs e)      // new cal but
        {
            if (check_method == 1)
            {
               
                    var dialog = new MessageDialog("New Calculation Simple Moving Average ");
                    await dialog.ShowAsync();
                
                quans.Clear();
                whiFor.Clear();
                sp1.Visibility = Visibility.Visible;
                wh.Visibility = Visibility.Collapsed;
                x = 0;
                check = false;
                which.Text = string.Empty;
                numOfPer.Text = string.Empty;
                quan.Text = string.Empty;
            }


            if (check_method == 2)
            {
                var dialog = new MessageDialog("New Calculation Exponential Smoothing ");
                await dialog.ShowAsync();

                Forecast.Clear();
                foreConstant = 0;
                quans.Clear();
                whiFor.Clear();

                wh.Visibility = Visibility.Collapsed;
                sp2.Visibility = Visibility.Visible;
                sp4.Visibility = Visibility.Visible;

                which.Text = string.Empty;
                quan.Text = string.Empty;
                firFore.Text = string.Empty;
                cons.Text = string.Empty;
              
            }

            //-------------------------------------------------
            if (check_method == 3)
            {
                var dialog = new MessageDialog("new cal method 3");
                await dialog.ShowAsync();
            }

        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(how));

        }


        //----------------------------------------------------------------------

    }
}
