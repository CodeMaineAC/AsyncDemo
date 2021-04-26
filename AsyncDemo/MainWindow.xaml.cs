using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            resultsWindow.Text = "";
        }

        /*
         * Code for the various buttons
         */

        private void Run_Sync_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            RunDownloadSync();

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += "Total Excution time: " + elapsedMs + " ms";

        }

        //set to async so that we can await on the asynchronous to finish, needs to be async in order to do this 
        private async void Run_Async_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadAsync();

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += "Total Excution time: " + elapsedMs + " ms";

        }

        //pretty much the same as Run_Async_Click but awaits on a different async method, RunDownloadParallelAsync()
        private async void Run_ParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadParallelAsync();

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += "Total Excution time: " + elapsedMs + " ms";
        }

        //Gets the Urls we want to get data on and puts them into a list
        private List<string> PrepData()
        {
            List<string> output = new List<string>();

            output.Add("https://www.google.com");
            output.Add("https://www.microsoft.com");
            output.Add("https://bangordailynews.com/");
            output.Add("https://www.mainerobotics.org/");
            output.Add("https://stackoverflow.com/");
            output.Add("https://xkcd.com/");
            output.Add("https://www.yahoo.com/");
            output.Add("https://store.steampowered.com/");
            output.Add("https://www.geeksforgeeks.org/");

            resultsWindow.Text = "";
            return output;
        }

        //Sets up the URLs to download data and then calls on the data to be outputed to the user
        private void RunDownloadSync()
        {
            List<string> websites = PrepData();

            foreach (string site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                ReportWebsiteInfo(results);
            }
        }

        //Similliar to RunDownloadSync() but can be called to run asynchronously, No real performance gain but other things can be done while it runs
        private async Task RunDownloadAsync()
        {
            List<string> websites = PrepData();

            foreach (string site in websites)
            {
                WebsiteDataModel results = await Task.Run(() => DownloadWebsite(site));
                ReportWebsiteInfo(results);
            }
        }

        //Sets up the downloads to run in parallel which improves speed
        private async Task RunDownloadParallelAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (string site in websites)
            {
                tasks.Add(DownloadWebsiteAsync(site));
            }

            WebsiteDataModel[] results = await Task.WhenAll(tasks);

            foreach (WebsiteDataModel item in results)
            {
                ReportWebsiteInfo(item);
            }
        }

        //Puts the data into the class WebsiteDataModel made for this project
        private WebsiteDataModel DownloadWebsite(string url)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = url;
            output.WebsiteData = client.DownloadString(url);

            return output;
        }

        //Similiar to DownloadWebsite() but can be called asynchronously and uses a built-in asynchronous method
        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string url)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = url;
            output.WebsiteData = await client.DownloadStringTaskAsync(url);

            return output;
        }

        //outputs results of the website downloads to the text box in the GUI
        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            resultsWindow.Text += "" + data.WebsiteUrl +
                " download: " + data.WebsiteData.Length + " characters long.\n";
        }
    }
}
