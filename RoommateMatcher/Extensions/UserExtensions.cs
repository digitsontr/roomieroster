using System;
using RoommateMatcher.Models;

namespace RoommateMatcher.Extensions
{
    public static class UserExtensions
    {
        public static IQueryable<AppUser> FilterByPreferences(
            this IQueryable<AppUser> users,
            AppUserPreferences preferences, 
            AppUser user)
        {
            var b = users.ToList();
            Console.WriteLine(users.ToList());

            var preferenceProperties = typeof(AppUserPreferences)
                .GetProperties();
            var byteProperties = preferenceProperties
                .Where(z => z.PropertyType == typeof(byte));
            var rangeProperties = preferenceProperties
                .Where(z => z.PropertyType == typeof(int) ||
                    z.PropertyType == typeof(float)
                );
            var addressProperties = typeof(AppUserAddress).GetProperties();

            foreach (var property in byteProperties)
            {
                switch (property.Name)
                {
                    case "SmokingAllowed":
                        users = users.Where(z => z.Status &&
                        (z.Preferences.SmokingAllowed ==
                        preferences.SmokingAllowed
                        || (preferences.SmokingAllowed == 2 ||
                        z.Preferences.SmokingAllowed == 2)));
                        break;
                    case "GuestsAllowed":
                        users = users.Where(z => z.Status &&
                        (z.Preferences.GuestsAllowed ==
                        preferences.GuestsAllowed
                        || (preferences.GuestsAllowed == 2 ||
                        z.Preferences.GuestsAllowed == 2)));
                        break;
                    case "PetsAllowed":
                        users = users.Where(z => z.Status &&
                        (z.Preferences.PetsAllowed == preferences.PetsAllowed
                        || (preferences.PetsAllowed == 2 ||
                        z.Preferences.PetsAllowed == 2)));
                        break;
                    case "GenderPref":
                        var c = users.ToList();

                        users = users.Where(z => (z.Status &&
                        (z.Preferences.GenderPref == preferences.GenderPref
                        && (z.Gender == user.Gender
                        || (z.Preferences.GenderPref == 2 &&
                        preferences.GenderPref == 2)))
                        || (preferences.GenderPref == 2 &&
                        z.Preferences.GenderPref == user.Gender) ||
                        (z.Preferences.GenderPref == user.Gender &&
                        (preferences.GenderPref == 2 ||
                        preferences.GenderPref == z.Gender))));

                        var d = users.ToList();
                        break;
                    case "ForeignersAllowed":
                        users = users.Where(z => z.Status &&
                        (z.Preferences.ForeignersAllowed ==
                        preferences.ForeignersAllowed ||
                        (preferences.ForeignersAllowed == 2 ||
                        z.Preferences.ForeignersAllowed == 2)));
                        break;
                    case "AlcoholAllowed":
                        users = users.Where(z => z.Status &&
                        (z.Preferences.AlcoholAllowed ==
                        preferences.AlcoholAllowed ||
                        (preferences.AlcoholAllowed == 2 ||
                        z.Preferences.AlcoholAllowed == 2)));
                        break;
                    case "Duration":
                        users = users.Where(z => z.Status &&
                        (z.Preferences.Duration == preferences.Duration ||
                        (preferences.Duration == 2 ||
                        z.Preferences.Duration == 2)));
                        break;
                    default:
                        break;
                }
            }
            
            foreach (var address in addressProperties)
            {
                switch (address.Name)
                {
                    case "Country":
                        users = users.Where(z => z.Status &&
                        (preferences.Address.Country == "" ||
                        z.Preferences.Address.Country ==
                        preferences.Address.Country));
                        break;
                    case "City":
                        users = users.Where(z => z.Status &&
                        (preferences.Address.City == "" ||
                        z.Preferences.Address.City ==
                        preferences.Address.City));
                        break;
                    case "District":
                        users = users.Where(z => z.Status &&
                        (preferences.Address.District == "" ||
                        z.Preferences.Address.District ==
                        preferences.Address.District));
                        break;
                    case "Neighborhood":
                        users = users.Where(z => z.Status &&
                        (preferences.Address.Neighborhood == "" ||
                        z.Preferences.Address.Neighborhood ==
                        preferences.Address.Neighborhood));
                        break;
                    default:
                        break;
                }
            }
         

                foreach (var property in rangeProperties)
            {
                switch (property.Name)
                {
                    case "BudgetMin":
                        users = users.Where(z => z.Preferences.BudgetMin
                        >= preferences.BudgetMin);
                        break;
                    case "AcceptableRoommatesMin":
                        users = users.Where(z => z.Preferences
                        .AcceptableRoommatesMin >=
                        preferences.AcceptableRoommatesMin
                        && z.Preferences.AcceptableRoommatesMin
                        <= preferences.AcceptableRoommatesMax);
                        break;
                    default:
                        break;
                }
            }

            return users;
        }
    }
}

