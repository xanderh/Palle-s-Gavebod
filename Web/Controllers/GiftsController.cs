using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Entities;
using System.Net.Http;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class GiftsController : Controller
    {
        // HTTP client used to get data from API
        private readonly HttpClient _httpClient;
        // The base URI for the web api
        private Uri BaseEndPoint { get; set; }

        public GiftsController()
        {
            // Set the port to whatever the API port is once you've started it once. Shouldn't change on restarts.
            BaseEndPoint = new Uri("https://localhost:44329/api/gifts");
            _httpClient = new HttpClient();
        }

        // GET: Gifts
        public async Task<IActionResult> Index()
        {
            // use HTTP client to read data from API. Move on once the headers have been read. Errors are caught slightly quicker this way.
            var response = await _httpClient.GetAsync(BaseEndPoint, HttpCompletionOption.ResponseHeadersRead);
            // Make sure that we got a success status code in the headers. Returns an exception (and 500 status code) if not successful
            response.EnsureSuccessStatusCode();
            // Turn the response body into a string
            var data = await response.Content.ReadAsStringAsync();
            // Treat the response body string as JSON, and deserialize it into a list of gifts
            return View(JsonConvert.DeserializeObject<List<Gift>>(data));
        }

        // GET: Gifts
        public async Task<IActionResult> GirlGifts()
        {
            var response = await _httpClient.GetAsync(BaseEndPoint.ToString() + "/Girl", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return View(JsonConvert.DeserializeObject<List<Gift>>(data));
        }

        // GET: Gifts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var response = await _httpClient.GetAsync(BaseEndPoint.ToString() + id, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var gift = JsonConvert.DeserializeObject<Gift>(data);
            if (gift == null)
            {
                return NotFound();
            }

            return View(gift);
        }

        // GET: Gifts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GiftNumber,Title,Description,CreationDate,BoyGift,GirlGift")] Gift gift)
        {
            if (ModelState.IsValid)
            {
                // Post the created gift as JSON to API. HttpClient handles serialization for us
                var response = await _httpClient.PostAsJsonAsync<Gift>(BaseEndPoint, gift);
                // Make sure we got a success, otherwise return 500
                response.EnsureSuccessStatusCode();
                // Redirect back to overview page
                return RedirectToAction(nameof(Index));
            }
            return View(gift);
        }

        private async Task<bool> GiftExists(int id)
        {
            var response = await _httpClient.GetAsync(BaseEndPoint, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var context = JsonConvert.DeserializeObject<List<Gift>>(data);
            return context.Any(e => e.GiftNumber == id);
        }
    }
}
