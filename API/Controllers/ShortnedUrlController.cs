using API.Infrastructure;
using API.Services;
using API.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/v1/[controller]")]
    public class ShortnedUrlController : Controller
    {
        private readonly ApiService _service;

        public ShortnedUrlController(ApiService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// получение списка всех сокращенных ссылок с количеством переходов
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<UrlKeyStat>> Get()
        {
            var uriMappings = await _service.GetAllMappingsAsync();

            return uriMappings.Select(mapping => new UrlKeyStat
            {
                HitCount = mapping.HitCount,
                Url = UrlHelper.AddShemeAndDomain(mapping.ShortenedKey)
            });
        }

        /// <summary>
        /// получение оригинала по сокращенной, с увеличением счетчика посещений
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <response code="200">Ссылка найдена</response>
        /// <response code="400">Неверно задан параметр запроса</response>
        /// <response code="404">Ссылка не найдена</response>
        [HttpGet("{url}", Name = "GetByKey")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(string url)
        {
            var validationResult = UrlHelper.IsUrlValid(WebUtility.UrlDecode(url));
            if (validationResult.isValid)
            {
                var result = await _service.ResolveByKey(validationResult.key);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result.Uri);
            }
            return BadRequest();
        }

        /// <summary>
        /// создание сокращенной ссылки по полной
        /// </summary>
        /// <response code="201">Ссылка создана</response>
        /// <response code="400">Неверно задан параметр запроса</response>
        /// <response code="409">Ссылка уже была создана</response>
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ValidateModel]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Request request)
        {
            var result = await _service.CreateAsync(new Uri(request.Url));

            if (result.createdWithoutConflict)
            {
                var uriMapping = result.mapping;
                var shortnedUrl = UrlHelper.AddShemeAndDomain(uriMapping.ShortenedKey);
                return CreatedAtRoute("GetByKey", new { url = shortnedUrl }, shortnedUrl);
            }
            return StatusCode((int)HttpStatusCode.Conflict);
        }
    }
}