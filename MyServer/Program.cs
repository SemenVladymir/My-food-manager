using HtmlAgilityPack;
using MyServer.BDConnection;
using MyServer.Models;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;
using System.Xml;

public class Program
{
    private static void Main(string[] args)
    {
        SaveDishesToSQL(ReadDishesFromSite());
        //UpdateDishesInSQL(ReadDishesFromSite());
        //ReadProductsFromATB();
        //ReadProductsFromVarus();
        //ReadProductsFromSilpo();
        int u = 0;
    }

    private static List<Dish> ReadDishesFromSite()
    {
        string menuURL = "https://shefkuhar.com.ua/";
        HtmlWeb newweb1 = new HtmlWeb();
        HtmlDocument htmlSnippet;
        List<string> hrefTags = new List<string>();
        string newpage = "";
        for (int j = 200; j <= 229; j++)  
        {
            Console.WriteLine($"Page {j}");
            if (j > 1)
                newpage = $"page/{j}/";
            htmlSnippet = newweb1.Load($"{menuURL}{newpage}");

            foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes("//div[@id='dle-content']/a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                hrefTags.Add(att.Value);
            }
        }
        Console.WriteLine("---------------------------------------------------------------------");
        List<Dish> dish = new List<Dish>();
        int f = 1;
        foreach (var item in hrefTags)
        {
            htmlSnippet = newweb1.Load(item);
            string name = htmlSnippet.DocumentNode.SelectSingleNode("//div[@class='title_bg']").InnerText.Replace("&#039;", "'");
            string classify = htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='provse1']")?.InnerText?.Replace("&#039;", "'");
            string mainPhotoURL = "";
            if (htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='fotochka']/a[@href]")!=null)
                mainPhotoURL = htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='fotochka']/a[@href]")?.Attributes["href"]?.Value?.Replace("&#58;", ":");
            string ingredients = "В описі рецепту";
            if (htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='ingredients']")!=null)
                ingredients = htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='ingredients']").InnerText.Replace("&#039;", "'");
            string time = htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='chas']")?.InnerText?.Replace("&#039;", "'");
            string keywords = htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='chas']/span[@itemprop='keywords']")?.InnerText?.Replace("&#039;", "'");
            string recipe = "";
            string photos = "";
            string description;
            if (htmlSnippet?.DocumentNode?.SelectNodes("//div[@class='shemka']/ol/li") != null)
            {
                description = htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='shemka']/p")?.InnerText?.Replace("&#039;", "'");
                foreach (HtmlNode node in htmlSnippet?.DocumentNode?.SelectNodes("//div[@class='shemka']/ol/li"))
                {
                    recipe += "- " + node.InnerText + "\n";
                }
                foreach (HtmlNode node in htmlSnippet?.DocumentNode?.SelectNodes("//div[@class='shemka']/ol/li/div/a[@href]"))
                {
                    photos += node.Attributes["href"].Value + "\n";
                }
            }
            else
            {
                if (htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='shemka']/p") != null)
                {
                    string[] ds = htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='shemka']/p").InnerHtml?.Replace("&#039;", "'").Split("<br>");
                    if (mainPhotoURL.Length == 0)
                    {
                        mainPhotoURL = htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='shemka']/div/a[@href]").Attributes["href"]?.Value?.Replace("&#58;", ":");
                        if (!mainPhotoURL.Contains("http"))
                            mainPhotoURL = "https://shefkuhar.com.ua" + mainPhotoURL;
                    }
                    if (ds[0] != "")
                        description = ds[0];
                    else if (ds.Length > 1 && ds[1] != "" && !ds[1].Contains("div"))
                        description = ds[1];
                    else if (ds.Length > 2)
                        description = ds[2];
                    else
                        description = "---";
                    foreach (HtmlNode node in htmlSnippet?.DocumentNode?.SelectNodes("//div[@class='shemka']/p"))
                    {
                        ingredients = node.InnerText.Contains("Інгредієнти")?node.InnerText.Replace(description, "").Replace("Інгредієнти:", ""): ingredients;
                        recipe += node.InnerText.Contains("Інгредієнти")?"": "- " + node.InnerText.Replace(description, "") + "\n";
                    }
                }
                else
                {
                    string[] ds = htmlSnippet?.DocumentNode?.SelectSingleNode("//div[@class='shemka']").InnerHtml?.Replace("&#039;", "'").Replace("\n", "").Split("<br>");
                    if (mainPhotoURL.Length == 0)
                    {
                        mainPhotoURL = ds.FirstOrDefault(ee => ee.Contains("http")).Substring(ds.FirstOrDefault(ee => ee.Contains("http")).IndexOf("http"), ds.FirstOrDefault(ee => ee.Contains("http")).IndexOf(".jpg")- ds.FirstOrDefault(ee => ee.Contains("http")).IndexOf("http")+4);
                        if (!mainPhotoURL.Contains("http"))
                            mainPhotoURL = "https://shefkuhar.com.ua" + mainPhotoURL;
                    }
                    if (ds[0] != "")
                        description = ds[0];
                    else if (ds.Length > 1 && ds[1]!="" && !ds[1].Contains("div"))
                        description = ds[1];
                    else if (ds.Length > 2)
                        description = ds[2];
                    else
                        description = "---";
                    //description = string.IsNullOrEmpty(string.IsNullOrEmpty(ds[0])? ds[1] : ds[0])? ds[2] : ds[3];
                    foreach (HtmlNode node in htmlSnippet?.DocumentNode?.SelectNodes("//div[@class='shemka']"))
                    {
                        recipe += "- " + node.InnerText.Replace(description, "") + "\n";
                    }
                }
                foreach (HtmlNode node in htmlSnippet?.DocumentNode?.SelectNodes("//div[@class='shemka']/div[@style='text-align:center;']/a[@href]"))
                {
                    if (!node.Attributes["href"].Value.Contains("http"))
                        photos += "https://shefkuhar.com.ua" + node.Attributes["href"].Value + "\n";
                }
            }
            dish.Add(new Dish {Name = name, Classification = classify, MainPhotoURL = mainPhotoURL, Ingredients = ingredients, Description = description.Replace("<strong>Інгредієнти:</strong>", ""), CookingTime = time, KeyWords = keywords,
                RecipeInstruction = recipe?.Replace("&#039;", "'"), RecipePhotoURL = photos?.Replace("&#058;", ":")
            }) ;
            Console.WriteLine($"Dish {f}");
            f++;
        }
        return dish;
    }

    private static List<Product> ReadProductsFromATB()      //ATB
    {
        //string[] myurl = {"https://www.atbmarket.com/catalog/289-ovochi", "https://www.atbmarket.com/catalog/288-frukti-yagodi", https://www.atbmarket.com/catalog/390-zelen, "https://www.atbmarket.com/catalog/298-gribi", "https://www.atbmarket.com/catalog/285-bakaliya",
        //"https://www.atbmarket.com/catalog/315-molochni-produkti", "https://www.atbmarket.com/catalog/325-khlibobulochni-virobi",
        //"https://www.atbmarket.com/catalog/343-m-yaso-ta-yaytsya", "https://www.atbmarket.com/catalog/360-kovbasa-i-m-yasni-delikatesi",
        //"https://www.atbmarket.com/catalog/353-riba-i-moreprodukti", "https://www.atbmarket.com/catalog/322-zamorozheni-produkti",
        //"https://www.atbmarket.com/catalog/292-alkogol-i-tyutyun", "https://www.atbmarket.com/catalog/294-napoi-bezalkogol-ni"};

        string[] myurl = { "https://www.atbmarket.com/catalog/298-gribi" };

        List<Product> ATBproducts = new List<Product>();
        int clasify = 5;
        foreach (string item in myurl)
        {
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine($"-----------------{item.Split('/')[4]}------------------------------");
            Console.WriteLine("---------------------------------------------------------------------");
            int page = 1;
            HtmlWeb newweb = new HtmlWeb();
            HtmlDocument newdoc = newweb.Load(item);
            while (newdoc != null)
            {
                //HtmlNodeCollection res = newdoc?.DocumentNode?.SelectNodes("//article[@class='  catalog-item js-product-container    ']");
                HtmlNodeCollection res = newdoc?.DocumentNode?.SelectNodes("//article");
                if (res == null)
                {
                    res = newdoc?.DocumentNode?.SelectNodes("//article[@class='  catalog-item js-product-container   product-age-checkout  catalog-item--alco ']");
                }
                //HtmlNodeCollection count = newdoc.DocumentNode.SelectNodes("//ul[@class='product-pagination__list']");
                //int it = count[0].InnerHtml.Split("</li>").Count()-3;
                foreach (HtmlNode node in res)
                {
                    string name;
                    if (node?.SelectSingleNode(".//div[@class='catalog-item__info']")?.InnerText == null)
                        continue;
                    else
                        name = node.SelectSingleNode(".//div[@class='catalog-item__info']").InnerText;
                    name = name.Replace("\n", "").Replace("  ", "").Replace("власна марка АТБ", "").Replace("і", "i").Replace("І", "I");
                    if (ATBproducts.Where(e => e.Name == name).Any())
                    {
                        newdoc = null;
                        continue;
                    }
                    else
                    {
                        try
                        {
                            string urlPhoto;
                            if (node?.SelectSingleNode(".//picture")?.InnerHtml != null)
                            {
                                string pic = node?.SelectSingleNode(".//picture")?.InnerHtml;
                                string[] pic1 = pic.Split('"');
                                urlPhoto = pic1[Array.FindIndex(pic1, 0, e => e.Contains("src=")) + 1];
                            }
                            else
                            {
                                urlPhoto = "‪C:\\Users\\User\\OneDrive\\Pictures\\WithoutPhoto.jpg";
                            }
                            string cost = node.SelectSingleNode(".//data[@class='product-price__top']").InnerText;
                            cost = cost.Replace("\n", "").Replace("  ", "").Replace(".", ",");
                            string dimention = cost.Substring(cost.IndexOf("грн"));
                            int len = cost.IndexOf("грн");
                            string scost = cost.Substring(0, len);
                            double dcost = Convert.ToDouble(scost);
                            ATBproducts.Add(new Product { Name = name, Price = dcost, PhotoURL = urlPhoto, Dimension = dimention, CategoryId = clasify, ProviderId = 1, IsAvailable=true });
                            Console.WriteLine($"{name}, cost-{cost}");
                        }
                        catch (Exception e) { Console.WriteLine(e); }
                    }
                }
                if (newdoc != null)
                {
                    page++;
                    newdoc = newweb.Load($"{item}?page={page}");
                }
            }
            SaveProductsToSQL(ATBproducts);
            clasify++;
        }
        return ATBproducts;
    }

    private static List<Product> ReadProductsFromVarus()    //VARUS
    {
        //string[] varus = { "https://varus.ua/frukti-ovochi-gorihi/ovochi-svizhi", "https://varus.ua/frukti-ovochi-gorihi/frukti-svizhi", "https://varus.ua/frukti-ovochi-gorihi/yagodi-svizhi",
        //                     "https://varus.ua/frukti-ovochi-gorihi/zelen-svizha", "https://varus.ua/frukti-ovochi-gorihi/gribi", "https://varus.ua/frukti-ovochi-gorihi/suhofrukti",
        //                    "https://varus.ua/frukti-ovochi-gorihi/gorihi-ta-nasinnya", "https://varus.ua/molochni-produkti/yaycya", "https://varus.ua/molochni-produkti/siri",
        //                    "https://varus.ua/molochni-produkti/moloko", "https://varus.ua/molochni-produkti/smetana", "https://varus.ua/molochni-produkti/maslo",
        //                    "https://varus.ua/molochni-produkti/yogurti", "https://varus.ua/molochni-produkti/kislomolochni-produkti", "https://varus.ua/molochni-produkti/deserti-molochni",
        //                    "https://varus.ua/molochni-produkti/vershki", "https://varus.ua/molochni-produkti/sirki", "https://varus.ua/molochni-produkti/zguschene-moloko",
        //                    "https://varus.ua/molochni-produkti/bezlaktozni-produkti", "https://varus.ua/myasni-virobi-ta-yaycya/myaso-svizhe", "https://varus.ua/myasni-virobi-ta-yaycya/napivfabrikati-myasni",
        //                    "https://varus.ua/myasni-virobi-ta-yaycya/shashlik-marinovaniy", "https://varus.ua/kolbasy-sosiski-delikatesy/kovbasi", "https://varus.ua/kolbasy-sosiski-delikatesy/sosiski-ta-sardelki",
        //                    "https://varus.ua/kolbasy-sosiski-delikatesy/narizki-ta-delikatesi", "https://varus.ua/riba-ta-moreprodukti/ikra-ribi", "https://varus.ua/riba-ta-moreprodukti/riba",
        //                    "https://varus.ua/riba-ta-moreprodukti/krabovi-palochki", "https://varus.ua/riba-ta-moreprodukti/moreprodukti", "https://varus.ua/riba-ta-moreprodukti/pasti-z-moreproduktiv",
        //                    "https://varus.ua/riba-ta-moreprodukti/ribni-napivfabrikati", "https://varus.ua/hlibobulochni-virobi/hlib", "https://varus.ua/hlibobulochni-virobi/hlibci",
        //                    "https://varus.ua/hlibobulochni-virobi/sushki-ta-suhari", "https://varus.ua/hlibobulochni-virobi/bulochni-virobi", "https://varus.ua/hlibobulochni-virobi/muchnye-kulinarnye-izdeliya",
        //                    "https://varus.ua/hlibobulochni-virobi/korzhi-ta-zagotovki", "https://varus.ua/bakaliya/makaronni-virobi", "https://varus.ua/bakaliya/krupi",
        //                    "https://varus.ua/bakaliya/cukor-sil-ta-pripravi", "https://varus.ua/bakaliya/stravi-shvidkogo-prigotuvannya", "https://varus.ua/bakaliya/maslo-roslinne",
        //                    "https://varus.ua/bakaliya/sousi", "https://varus.ua/bakaliya/varennya-pasti-med", "https://varus.ua/bakaliya/konservi",
        //                    "https://varus.ua/bakaliya/konservaciya", "https://varus.ua/bakaliya/produkti-dlya-sushi", "https://varus.ua/bakaliya/produkti-dlya-vipichki", "https://varus.ua/bakaliya/soevi-produkti"};

        string[] varus = { "https://varus.ua/frukti-ovochi-gorihi/gribi", "https://varus.ua/frukti-ovochi-gorihi/suhofrukti",
                            "https://varus.ua/frukti-ovochi-gorihi/gorihi-ta-nasinnya", "https://varus.ua/molochni-produkti/yaycya", "https://varus.ua/molochni-produkti/siri",
                            "https://varus.ua/molochni-produkti/moloko", "https://varus.ua/molochni-produkti/smetana", "https://varus.ua/molochni-produkti/maslo",
                            "https://varus.ua/molochni-produkti/yogurti", "https://varus.ua/molochni-produkti/kislomolochni-produkti", "https://varus.ua/molochni-produkti/deserti-molochni",
                            "https://varus.ua/molochni-produkti/vershki", "https://varus.ua/molochni-produkti/sirki", "https://varus.ua/molochni-produkti/zguschene-moloko",
                            "https://varus.ua/molochni-produkti/bezlaktozni-produkti", "https://varus.ua/myasni-virobi-ta-yaycya/myaso-svizhe", "https://varus.ua/myasni-virobi-ta-yaycya/napivfabrikati-myasni",
                            "https://varus.ua/myasni-virobi-ta-yaycya/shashlik-marinovaniy", "https://varus.ua/kolbasy-sosiski-delikatesy/kovbasi", "https://varus.ua/kolbasy-sosiski-delikatesy/sosiski-ta-sardelki",
                            "https://varus.ua/kolbasy-sosiski-delikatesy/narizki-ta-delikatesi", "https://varus.ua/riba-ta-moreprodukti/ikra-ribi", "https://varus.ua/riba-ta-moreprodukti/riba",
                            "https://varus.ua/riba-ta-moreprodukti/krabovi-palochki", "https://varus.ua/riba-ta-moreprodukti/moreprodukti", "https://varus.ua/riba-ta-moreprodukti/pasti-z-moreproduktiv",
                            "https://varus.ua/riba-ta-moreprodukti/ribni-napivfabrikati", "https://varus.ua/hlibobulochni-virobi/hlib", "https://varus.ua/hlibobulochni-virobi/hlibci",
                            "https://varus.ua/hlibobulochni-virobi/sushki-ta-suhari", "https://varus.ua/hlibobulochni-virobi/bulochni-virobi", "https://varus.ua/hlibobulochni-virobi/muchnye-kulinarnye-izdeliya",
                            "https://varus.ua/hlibobulochni-virobi/korzhi-ta-zagotovki", "https://varus.ua/bakaliya/makaronni-virobi", "https://varus.ua/bakaliya/krupi",
                            "https://varus.ua/bakaliya/cukor-sil-ta-pripravi", "https://varus.ua/bakaliya/stravi-shvidkogo-prigotuvannya", "https://varus.ua/bakaliya/maslo-roslinne",
                            "https://varus.ua/bakaliya/sousi", "https://varus.ua/bakaliya/varennya-pasti-med", "https://varus.ua/bakaliya/konservi",
                            "https://varus.ua/bakaliya/konservaciya", "https://varus.ua/bakaliya/produkti-dlya-sushi", "https://varus.ua/bakaliya/produkti-dlya-vipichki", "https://varus.ua/bakaliya/soevi-produkti"};

        List<Product> VARUSproducts = new List<Product>();
        int clasify = 5;
        foreach (string item in varus)
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------------");
            Console.WriteLine($"-----------------{item.Split('/')[4]}------------------------------");
            Console.WriteLine("--------------------------------------------------------------------------------------------------");
            int page = 1;
            HtmlWeb newweb = new HtmlWeb();
            HtmlDocument newdoc = newweb.Load(item);
            while (newdoc != null)
            {
                HtmlNodeCollection res = newdoc?.DocumentNode?.SelectNodes("//div[@class='sf-product-card__wrapper m-category-list__item']");
                if (res == null)
                {
                    newdoc = null;
                    continue;
                }
                //HtmlNodeCollection count = newdoc.DocumentNode.SelectNodes("//ul[@class='pagination']");
                //int it = count[0].InnerHtml.Split("</a>").Count() - 3;
                foreach (HtmlNode node in res)
                {
                    string name = node.SelectSingleNode(".//p[@class='sf-product-card__title']").InnerText;
                    if (node?.SelectSingleNode(".//div[@class='sf-product-card__out-of-stock']")?.InnerText == null)
                    {


                        name = name.Replace("\n", "").Replace("  ", "").Replace("власна марка АТБ", "").Replace("і", "i").Replace("І", "I");
                        if (VARUSproducts.Where(e => e.Name == name).Any() || page == 30)
                        {
                            newdoc = null;
                            continue;
                        }
                        else
                        {
                            try
                            {
                                string urlPhoto;
                                string dimention;
                                if (node?.SelectSingleNode(".//div[@class='sf-image sf-product-card__image']")?.InnerHtml != null)
                                {
                                    string pic = node?.SelectSingleNode(".//div[@class='sf-image sf-product-card__image']")?.InnerHtml;
                                    string[] pic1 = pic.Split('"');
                                    urlPhoto = pic1[Array.FindIndex(pic1, 0, e => e.Contains("src=")) + 1];
                                }
                                else
                                {
                                    urlPhoto = "‪C:\\Users\\User\\OneDrive\\Pictures\\WithoutPhoto.jpg";
                                }
                                string cost = node.SelectSingleNode(".//div[@class='sf-price sf-product-card__price']").InnerText;
                                cost = cost.Replace("\n", "").Replace("  ", "").Replace(".", ",");
                                if (node?.SelectSingleNode(".//p[@class='sf-product-card__quantity']")?.InnerText != null)
                                {
                                    dimention = node?.SelectSingleNode(".//p[@class='sf-product-card__quantity']")?.InnerText.Replace(" за 1 ", "").Replace(" ", "");
                                }
                                else
                                {
                                    dimention = "нема даних";
                                }
                                //string quantity = node?.SelectSingleNode(".//p[@class='sf-product-card__quantity']")?.InnerText;
                                //string dimention = cost.Substring(cost.IndexOf("грн"));
                                int len = cost.IndexOf("грн");
                                string scost = cost.Substring(0, len);
                                string[] tmp = scost.Split(' ');
                                double dcost;
                                if (tmp.Length > 2)
                                {
                                    dcost = Convert.ToDouble(tmp[2]);
                                }
                                else
                                {
                                    dcost = Convert.ToDouble(scost);
                                }
                                VARUSproducts.Add(new Product { Name = name, Price = dcost, PhotoURL = urlPhoto, Dimension = "грн/"+dimention, CategoryId = clasify, ProviderId = 3, IsAvailable=true });
                                Console.WriteLine($"{name}, cost-{dcost} {dimention}");
                            }
                            catch (Exception e) { Console.WriteLine(e); }
                        }
                    }
                }
                if (newdoc != null)
                {
                    page++;
                    newdoc = newweb.Load($"{item}?page={page}");
                }
            }
            SaveProductsToSQL(VARUSproducts);
            clasify++;
        }
        return VARUSproducts;
    }

    private static List<Product> ReadProductsFromSilpo()       //SILPO
    {
        List<Product> products = new List<Product>();
        string[] silpo = { "https://shop.silpo.ua/category/ovochi-378", "https://shop.silpo.ua/category/m-iaso-4411",
                           "https://shop.silpo.ua/category/ryba-4430", "https://shop.silpo.ua/category/m-iaso-kovbasni-vyroby-316"};

        List<Product> SILPOproducts = new List<Product>();
        int clasify = 1;
        foreach (string item in silpo)
        {
            int page = 1;
            HtmlWeb newweb = new HtmlWeb();
            HtmlDocument newdoc = newweb.Load(item);
            while (newdoc != null)
            {
                //HtmlNodeCollection res = newdoc?.DocumentNode?.SelectNodes("//div[@class='lazyload-wrapper ']");
                HtmlNodeCollection res = newdoc?.DocumentNode?.SelectNodes("//div");
                if (res == null)
                {
                    res = newdoc?.DocumentNode?.SelectNodes("//article[@class='  catalog-item js-product-container   product-age-checkout  catalog-item--alco ']");
                }
                HtmlNodeCollection count = newdoc.DocumentNode.SelectNodes("//ul[@class='pagination']");
                int it = count[0].InnerHtml.Split("</a>").Count() - 3;
                foreach (HtmlNode node in res)
                {
                    string name = node.SelectSingleNode(".//div[@class='catalog-item__info']").InnerText;
                    name = name.Replace("\n", "").Replace("  ", "").Replace("власна марка АТБ", "").Replace("і", "i").Replace("І", "I");
                    if (SILPOproducts.Where(e => e.Name == name).Any())
                    {
                        newdoc = null;
                        continue;
                    }
                    else
                    {
                        try
                        {
                            string urlPhoto;
                            if (node?.SelectSingleNode(".//picture")?.InnerHtml != null)
                            {
                                string pic = node?.SelectSingleNode(".//picture")?.InnerHtml;
                                string[] pic1 = pic.Split('"');
                                urlPhoto = pic1[Array.FindIndex(pic1, 0, e => e.Contains("src=")) + 1];
                            }
                            else
                            {
                                urlPhoto = "‪C:\\Users\\User\\OneDrive\\Pictures\\WithoutPhoto.jpg";
                            }
                            string cost = node.SelectSingleNode(".//data[@class='product-price__top']").InnerText;
                            cost = cost.Replace("\n", "").Replace("  ", "").Replace(".", ",");
                            string dimention = cost.Substring(cost.IndexOf("грн"));
                            int len = cost.IndexOf("грн");
                            string scost = cost.Substring(0, len);
                            double dcost = Convert.ToDouble(scost);
                            SILPOproducts.Add(new Product { Name = name, Price = dcost, PhotoURL = urlPhoto, Dimension = dimention, CategoryId = clasify, ProviderId = 2, IsAvailable=true });
                            Console.WriteLine($"{name}, cost-{cost}");
                        }
                        catch (Exception e) { Console.WriteLine(e); }
                    }
                }
                if (newdoc != null)
                {
                    page++;
                    newdoc = newweb.Load($"{item}?page={page}");
                }
            }
            SaveProductsToSQL(SILPOproducts);
            clasify++;
        }
        return products;
    }   //Поки не працює, треба доробити

    private static void SaveDishesToSQL(List<Dish> dishes)
    {
        using (BDContext context = new BDContext())
        {
            context.AddRange(dishes);
            int count = context.SaveChanges();
            if (count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{count} dishes added to the SQL-Base");
            }
        }
    }

    private static void SaveProductsToSQL(List<Product> products)
    {
        using (BDContext context = new BDContext())
        {
            context.AddRange(products);
            int count = context.SaveChanges();
            if (count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{count} products added to the SQL-Base");
            }
        }
    }

    private static void UpdateDishesInSQL(List<Dish> dishes)
    {
        using (BDContext context = new BDContext())
        {
            context.UpdateRange(dishes);
            int count = context.SaveChanges();
            if (count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{count} dishes updated in the SQL-Base");
            }
        }
    }

    private static void UpdateProductsInSQL(List<Product> products)
    {
        using (BDContext context = new BDContext())
        {
            context.UpdateRange(products);
            int count = context.SaveChanges();
            if (count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{count} products updated in the SQL-Base");
            }
        }
    }

    private List<Dish> ReadDishesFromSQL()
    {
        List<Dish> dishes;
        using (BDContext context = new BDContext())
        {
            dishes = new List<Dish>(context.Dishes.ToList());
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"{dishes.Count} dishes read from SQL-Base");
        return dishes;
    }

    private List<Product> ReadProductsFromSQL()
    {
        List<Product> products;
        using (BDContext context = new BDContext())
        {
            products = new List<Product>(context.Products.ToList());
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"{products.Count} products read from SQL-Base");
        return products;
    }


}