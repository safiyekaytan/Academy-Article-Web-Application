using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;
using yazlab2_1.Models;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using OpenQA.Selenium.Interactions;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;
using yazlab2_1.Data;
using Microsoft.AspNetCore.Http;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Nest;
using ConnectionSettings = Nest.ConnectionSettings;
using DnsClient.Protocol;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium.DevTools.V120.Overlay;





namespace yazlab2_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public MongoDBD manager = new MongoDBD();
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<Article> search)
        {
            ViewBag.oner = null;
            List<string> listAuthor = new List<string>();
            List<string> listDate= new List<string>();
            List<string> listHeader = new List<string>();
            List<string> listKeywords = new List<string>();
            List<string> listTypes = new List<string>();
            List<string> listPublishedArea = new List<string>();
            List<string> listAbstract = new List<string>();
            List<string> listCitedCount = new List<string>();
            List<string> listDoiNumber = new List<string>();
            List<string> listSearchKeywords = new List<string>();
            List<string> listUrl = new List<string>();
            List<List<string>> listReferences = new List<List<string>>();
            List<string> listDownloadLink = new List<string>();
            string aratilan = search[0].keywordsSearched;


            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless"); // Headless mode eklenir


            // WebDriver'ý baþlat
            IWebDriver driver = new ChromeDriver(options);

            try
            {
                // DergiPark'ýn web sitesine git
                driver.Navigate().GoToUrl("https://dergipark.org.tr/");
                // Arama kutusunu bul ve "makale" yazýsýný gönder
                IWebElement searchBox = driver.FindElement(By.Id("search_term"));
                searchBox.SendKeys(aratilan);
                searchBox.SendKeys(Keys.Enter);

                try
                {
                    var alert = driver.FindElements(By.CssSelector("i.flaticon-warning"));
                    if(alert.Count != 0)
                    {
                        //kelime yanlýþ girilmiþ!
                        //o kelimeyi googleye yaz doðrusunu al ve kullanýcýya öner, bunu mu girmek istedin de

                        driver.Navigate().GoToUrl("https://www.google.com/");
                        // Arama kutusunu bul ve "makale" yazýsýný gönder
                        IWebElement searchBox2 = driver.FindElement(By.Name("q"));
                        searchBox2.SendKeys(aratilan);
                        searchBox2.SendKeys(Keys.Enter);

                        try
                        {
                            IWebElement hatavar = driver.FindElement(By.TagName("omnient-visibility-control"));
                            if(hatavar != null)
                            {
                                IWebElement oner = hatavar.FindElement(By.TagName("p")).FindElement(By.TagName("a")).FindElement(By.TagName("b")).FindElement(By.TagName("i"));
                                Console.WriteLine("Yanlýþ yazýlmýþ olabilir, önerilen kelime: " +oner.Text );

                                var dataa = await manager.GetDataAsync();
                                string aaa = oner.Text;
                                ViewBag.oner = aaa;
                                return View(dataa);
                            }


                            
                        }
                        catch (NoSuchElementException)
                        {
                            Console.WriteLine("Öneri bulunamadý.");
                        }
                    }
                }
                catch
                {

                }

                // H5 etiketlerini ve class'ý "card-title" olanlarý bulma
                var h5Elements = driver.FindElements(By.CssSelector("h5.card-title"));

                // Her bir H5 etiketindeki baðlantýlarý bulma ve yeni sekmede açma
                int i = 0;
                foreach (var h5Element in h5Elements)
                {
                    if(i < 10)
                    {
                        var link = h5Element.FindElement(By.TagName("a"));
                        var url = link.GetAttribute("href");
                        // Baðlantýyý yeni sekmede açma
                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                        js.ExecuteScript("window.open(arguments[0]);", url);
                        i++;
                    }
                   
                }

                // Tüm pencereleri al
                var windows = driver.WindowHandles;
                string anaSekmeHandle = driver.CurrentWindowHandle;
                driver.SwitchTo().Window(anaSekmeHandle); // Ana sekme aktif hale getirilir
                driver.Close(); // Ana sekme kapatýlýr
                // Her bir pencere için iþlem yap
                foreach (var window in windows)
                {
                    
                    
                        try
                        {
  // Pencereye geç
                        driver.SwitchTo().Window(window);

                        // Yüklenene kadar bekle
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
                        wait.Until(ExpectedConditions.UrlToBe(driver.Url));

                             Thread.Sleep(1000);
                     

                        try
                        {
                            IWebElement author = driver.FindElement(By.XPath("//*[@class=\"tab-pane active \"]/p"));
                            listAuthor.Add(author.Text);
                        }
                        catch (Exception ex)
                        {
                            listAuthor.Add("Belirtilmemis");
                        }

                        try
                        {
                            IWebElement date = driver.FindElement(By.XPath("//*[@class=\"tab-pane active \"]/span"));
                            string[] dateString = date.Text.Split(",");
                            string[] dateString2 = dateString[3].Split(" ");

                            listDate.Add(dateString2[1]);
                        }
                        catch (Exception ex)
                        {
                            listDate.Add("Belirtilmemis");
                        }

                        try
                        {
                            IWebElement header = driver.FindElement(By.XPath("//*[@class=\"tab-pane active \"]")).FindElement(By.ClassName("h3")).FindElement(By.TagName("h3"));
                            listHeader.Add(header.Text);
                        }
                        catch (Exception ex)
                        {
                            listHeader.Add("Belirtilmemis");
                        }

                        try
                        {
                            IWebElement keywords = driver.FindElement(By.XPath("//*[@class=\"tab-pane active \"]")).FindElement(By.ClassName("article-keywords"));
                            listKeywords.Add(keywords.Text);
                        }
                        catch (Exception ex)
                        {
                            listKeywords.Add("Belirtilmemis");
                        }

                        try
                        {
                            IWebElement types = driver.FindElement(By.XPath("//*[@id=\"article-main-portlet\"]")).FindElement(By.ClassName("kt-portlet__head")).FindElement(By.ClassName("kt-portlet__head-label")).FindElement(By.ClassName("kt-portlet__head-title")).FindElement(By.ClassName("kt-font-bold"));
                            listTypes.Add(types.Text);
                        }
                        catch (Exception ex)
                        {
                            listTypes.Add("Belirtilmemis");
                        }

                        try
                        {
                            IWebElement publishedArea = driver.FindElement(By.XPath("//*[@id=\"journal-title\"]"));
                            listPublishedArea.Add(publishedArea.Text);
                        }
                        catch (Exception ex)
                        {
                            listPublishedArea.Add("Belirtilmemis");
                        }

                        try
                        {
                            string Aabstract = null;
                            ReadOnlyCollection<IWebElement> AabstractElements = driver.FindElement(By.XPath("//*[@class=\"tab-pane active \"]")).FindElement(By.ClassName("article-abstract")).FindElements(By.CssSelector("p, div"));
                            foreach (IWebElement abstractElement in AabstractElements)
                            {
                                // Her bir <p> etiketinin içeriðini almak için abstractElement.Text'i kullanabilirsiniz.
                                Aabstract += abstractElement.Text;
                            }
                            listAbstract.Add(Aabstract);
                        }
                        catch (Exception ex)
                        {
                            listAbstract.Add("Belirtilmemis");
                        }

                        try
                        {
                            IWebElement downloadLink = driver.FindElement(By.Id("article-toolbar")).FindElement(By.TagName("a"));
                            string hrefDownload = downloadLink.GetAttribute("href");
                            listDownloadLink.Add(hrefDownload);
                        }
                        catch (Exception ex)
                        {
                            listDownloadLink.Add("Belirtilmemis");
                        }
                        listSearchKeywords.Add(aratilan);

                        try
                        {
                            IWebElement references = driver.FindElement(By.XPath("//*[@class=\"tab-pane active \"]")).FindElement(By.ClassName("article-citations")).FindElement(By.TagName("div")).FindElement(By.TagName("ul"));
                            string[] referencesString = references.Text.Split("\r\n");
                            List<string> referencesStringList = new List<string>();

                            foreach (string referenceString in referencesString)
                                referencesStringList.Add(referenceString);
                            
                            listReferences.Add(referencesStringList);
                        }
                        catch (Exception ex)
                        {
                            List<string> referencesStringList = new List<string>();
                            listReferences.Add(referencesStringList);
                        }


                        try
                        {
                           IWebElement citedCount = driver.FindElement(By.XPath("//*[@class=\"tab-pane active \"]")).FindElement(By.ClassName("article-doi")).FindElement(By.ClassName("mt-3")).FindElement(By.TagName("a"));
                           string[] citedCountString = citedCount.Text.Split(":");
                           listCitedCount.Add(citedCountString[1]);
                        }
                        catch (Exception ex)
                        {
                           listCitedCount.Add("0");
                        }

                        try
                        {
                           IWebElement doiNumber = driver.FindElement(By.XPath("//*[@class=\"tab-pane active \"]")).FindElement(By.ClassName("article-doi")).FindElement(By.TagName("a"));
                           string[] doiString = doiNumber.Text.Split("/");
                           listDoiNumber.Add(doiString[3]);
                        }

                        catch (Exception ex)
                        {
                            listDoiNumber.Add("doi numarasi belirtilmemis");
                        }

                            string currentUrl = driver.Url;
                            listUrl.Add(currentUrl);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                }

            }
            finally
            {
                for(int i = 0; i < listDoiNumber.Count; i++)
                {
                    string format = "dd.MM.yyyy";
                    DateTime dateTime;
                    if (DateTime.TryParseExact(listDate[i], format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime))
                    {
                        Console.WriteLine("Baþarýyla dönüþtürüldü: " + dateTime);
                    }

                    try {
                        Article article = new Article()
                        {
                            publishName = listHeader[i],
                            authorsName = listAuthor[i],
                            publishType = listTypes[i],
                            publishDate = dateTime,
                            publishArea = listPublishedArea[i],
                            keywordsSearched = listSearchKeywords[i],
                            keywordsArticle = listKeywords[i],
                            summary = listAbstract[i],
                            references = listReferences[i],
                            numberOfCitations = listCitedCount[i],
                            doiNumber = listDoiNumber[i],
                            urlAddress = listUrl[i],
                            downloadLink = listDownloadLink[i]
                        };

                      


                        
                        await manager.InsertDataAsync(article);
                        // Veri ekleme
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }


                }
                    



            }
            // WebDriver'ý kapat
                 driver.Quit();

           
            // Veri çekme
            var data = await manager.GetDataAsync();

            return View(data);
        }
         static async Task   Aaaard()
        {
            var uri = new Uri("mongodb://localhost:27017");
            var settings = new ConnectionSettings(uri)
                .DefaultIndex("ArticleCollection4"); // Elasticsearch'de hedeflenen index adý

            var client = new ElasticClient(settings);

            // Elasticsearch'ten tüm belgeleri çek
            var searchResponse = await client.SearchAsync<Article>(s => s
                .MatchAll()
                .Size(1000)); // Çekilecek maksimum belge sayýsý

            if (searchResponse.IsValid)
            {
                var articles = searchResponse.Documents;
                foreach (var article in articles)
                {
                    Console.WriteLine($"Title: {article.authorsName}, Content: {article.summary}");
                }
            }
            else
            {
                Console.WriteLine($"Hata: {searchResponse.OriginalException.Message}");
            }
            
        }

       
        public class MongoDBVeriCekme
        {
            private IMongoCollection<Article> _yayinlarKoleksiyonu;

            public MongoDBVeriCekme(string connectionString, string databaseName)
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _yayinlarKoleksiyonu = database.GetCollection<Article>("ArticleCollection4");
            }

            public async Task<List<Article>> VeriCekAsync()
            {
                var belgeler = await _yayinlarKoleksiyonu.Find(_ => true).ToListAsync();
                return belgeler;
            }
        }

        public class MongoDBVeriEkleme
        {
            private IMongoCollection<Article> _yayinlarKoleksiyonu;

            public MongoDBVeriEkleme(string connectionString, string databaseName)
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _yayinlarKoleksiyonu = database.GetCollection<Article>("ArticleCollection4");
            }

            public async Task VeriEkleAsync(Article yayin)
            {
                await _yayinlarKoleksiyonu.InsertOneAsync(yayin);
            }
        }
        public async Task<IActionResult> Index()
        {
            
            // Veri çekme
            var data = await manager.GetDataAsync();
            
            return View(data);
        }

        public async Task<string> getHeader(string searchString)
        {

            if(searchString == null)
            {
                return null;
            }
            else
            {
                try
                {
                    Dictionary<string, int> searcingSkor = new Dictionary<string, int>();

                    List<string> headerStringList = new List<string>();
                    var data = await manager.GetDataAsync();

                    foreach (var item in data)
                    {
                        string[] parcala = item.publishName.Split(" ");
                        foreach (var item2 in parcala)
                        {
                            if (!headerStringList.Contains(item2))
                            {
                                headerStringList.Add(item2);
                            }
                        }
                    }

                    Dictionary<string, int> searcing = new Dictionary<string, int>();
                    int tempMax = 0;
                    foreach (var item in headerStringList)
                    {
                        int temp = 0;
                        foreach (var item1 in searchString)
                        {
                            if (item.ToLower().Contains(item1.ToString().ToLower()))
                            {
                                temp++;

                            }
                        }

                        searcing.Add(item, temp);
                        if (temp > tempMax)
                        {
                            tempMax = temp;
                        }
                    }

                    //tempMax === en çok tekrar eden kelimeye artý puan
                    foreach (var item in searcing)
                    {
                        if (item.Value == tempMax)
                        {
                            //en çok ortak harf buluna
                            searcingSkor.Add(item.Key, item.Value + 1);
                        }
                        else
                        {

                            searcingSkor.Add(item.Key, item.Value);
                        }
                    }

                    foreach (var item in searcingSkor)
                    {
                        if (item.Key.Length == searchString.Length)
                        {
                            //uzunlugu aynýysa

                            int value = searcingSkor[item.Key];
                            searcingSkor[item.Key] = value + 3;
                        }
                        else
                        {
                        }
                    }


                    foreach (var item in searcingSkor)
                    {
                        //deer lenght = 4; kombinasyon = 3.  i = 0, 1, 2.  substring(0,  2),  (1, 3)
                        int kombinasyon = searchString.Length - 1;
                        for (int i = 0; i < kombinasyon; i++)
                        {
                            string aranan = searchString.Substring(i, 2).ToLower();
                            string arayan = item.Key.ToLower();
                            if (arayan.Contains(aranan))
                            {
                                int value = searcingSkor[item.Key];
                                searcingSkor[item.Key] = value + 1;
                            }

                        }
                    }
                    int max = 0;
                    string vocabulary = null;
                    foreach (var item in searcingSkor)
                    {
                        if (item.Value > max)
                        {
                            max = item.Value;
                        }
                    }

                    foreach (var item in searcingSkor)
                    {
                        if (item.Value == max)
                        {
                            vocabulary = item.Key;
                        }
                    }

                    return vocabulary;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("hata oldu safiye:  " + ex.Message);
                    return null;
                }
            }
           
           
        }
        public async Task<IActionResult> Privacy(ObjectId id)
        {

            var data = await manager.GetDataAsync();
            Article article = new Article();
            foreach (var item in data)
            {
                if(item.Id == id)
                {
                    article = item;
                }
            }
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Download(string link)
        {
             ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless"); // Headless mode ekle
            options.AddUserProfilePreference("download.default_directory", @"C:\Users\safiy\Masaüstü");
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("download.directory_upgrade", true);
            options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);

            IWebDriver driver = new ChromeDriver(options);

            try
            {
               
                driver.Navigate().GoToUrl(link);

                Thread.Sleep(10000); 
                 
                driver.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           return RedirectToAction("Index");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
