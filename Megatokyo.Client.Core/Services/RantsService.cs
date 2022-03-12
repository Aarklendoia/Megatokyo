using AutoMapper;
using Megatokyo.Client.Core.DTO;
using Megatokyo.Client.Core.Mappings;
using Megatokyo.Client.Core.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Megatokyo.Client.Core.Services
{
    public class RantsService : IRantsService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public RantsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            MapperConfiguration config = new (cfg => {
                cfg.AddProfile<RantMappingProfile>();
            });
            _mapper = config.CreateMapper();

        }

        public async Task<IEnumerable<Rant>> GetAllRants()
        {
            IEnumerable<RantDTO> rants = new List<RantDTO>();
            try
            {
                HttpResponseMessage data = await _httpClient.GetAsync(ApiUrlConstants.GetAllRants); // .NET 6 : IEnumerable<RantDTO> rants = await httpClient.GetFromJsonAsync<RantDTO>(ApiUrlConstants.GetAllRants, cancellationToken);
                if (data.IsSuccessStatusCode)
                {
                    string jsonResponse = await data.Content.ReadAsStringAsync();
                    if (jsonResponse != null)
                        rants = JsonConvert.DeserializeObject<IEnumerable<RantDTO>>(jsonResponse);
                    return _mapper.Map<IEnumerable<Rant>>(rants);
                }
                return _mapper.Map<IEnumerable<Rant>>(rants);
            }
            catch
            {
                return _mapper.Map<IEnumerable<Rant>>(rants);
            }
        }

        public async Task<Rant> GetRantById(int id)
        {
            RantDTO rant = null;

            HttpResponseMessage data = await _httpClient.GetAsync(ApiUrlConstants.GetRant + "/" + id.ToString()); // .NET 6 : ApiUrlConstants.GetRant.AppendPathSegment(id)
            string jsonResponse = await data.Content.ReadAsStringAsync();
            if (jsonResponse != null)
                rant = JsonConvert.DeserializeObject<RantDTO>(jsonResponse);
            return _mapper.Map<Rant>(rant) ?? new();
        }
    }
}
