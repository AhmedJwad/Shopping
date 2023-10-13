using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;

namespace Shopping.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
           _context = context;
        }
        public async Task<IEnumerable<SelectListItem>> GetComboCategoriesAsync()
        {
            List<SelectListItem> list = await _context.Categories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = $"{x.Id}",
            }).OrderBy(x => x.Text).ToListAsync();

            list.Insert(0, new SelectListItem {
             Text= "{Select a category.}",
             Value="0",
            });
            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboCitiesAsync(int stateId)
        {
            List<SelectListItem> list = await _context.Cities.Where(x => x.state.Id == stateId).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = $"{x.Id}"
            }).OrderBy(x => x.Text).ToListAsync();
            list.Insert(0, new SelectListItem
            {
                Text = "{Select a city.}",
                Value = "0",
            });
            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboCountriesAsync()
        {
           List<SelectListItem> list=await _context.Countries.Select(x=> new SelectListItem
           {
               Text=x.Name,
               Value=$"{x.Id}",
           }).OrderBy(x=>x.Text).ToListAsync();
            list.Insert(0, new SelectListItem
            {
                Text = "{Select a country.}",
                Value = "0",
            });
            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboStatesAsync(int countryId)
        {
            List<SelectListItem>LIST=await _context.states.Where(X=>X.country.Id == countryId)
                .Select(X=> new SelectListItem
                {
                    Text=X.Name,
                    Value =$"{X.Id}"


                }).OrderBy(x=>x.Text).ToListAsync();
            LIST.Insert(0, new SelectListItem
            {
                Text = "{Select a states.}",
                Value = "0",
            });
            return LIST;

        }
    }
}
