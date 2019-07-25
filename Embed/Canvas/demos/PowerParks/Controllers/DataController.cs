using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PowerParks.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private static Park[] _parks =
        {
            new Park{Key="1", Name="Yosemite", Address="9035 Village Dr, Yosemite National Park, CA 95389", Notes= "Yosemite is the best"},
            new Park{Key="2", Name="Acadia", Address="20 Mcfarland Hill Dr, Bar Harbor, ME 04609", Notes= "Have to visit Acadia this year"},
            new Park{Key="3", Name="Channel Islands", Address="1901 Spinnaker Dr, Ventura, CA 93001", Notes= "Planning to take a boat here"},
            new Park{Key="4", Name="Pinnacles", Address="5000 Highway 146, Paicines, CA 95043", Notes= "Do this right after motorcyle trip"},
            new Park{Key="5", Name="Grand Canyon", Address="8 S Entrance Rd, Grand Canyon Village, AZ 86023", Notes= "Dare to hike this year"},
            new Park{Key="6", Name="Denali", Address="Mile 237 Hwy 3, Denali Park, AK 99755", Notes= "Plan on meditating here"},
            new Park{Key="7", Name="Kenai Fjords", Address="Kenai Peninsula Borough, AK", Notes= "Dont forget the DSLR"},
            new Park{Key="8", Name="Great Smoky Mountains", Address="107 Park Headquarters Rd, Gatlinburg, TN 37738", Notes= "The home of Barkley Marathon"},
            new Park{Key="9", Name="Olympic", Address=" 600 E Park Ave, Port Angeles, WA 98362", Notes= "Explore this summer on motorcycle"},
            new Park{Key="10", Name="Saguaro", Address="3693 S Old Spanish Trl, Tucson, AZ 85730", Notes= "Perfect during fall?"},
            new Park{Key="11", Name="Arches", Address="PO Box 907, Moab, UT 84532", Notes= "Not sure what the right time is"},
            new Park{Key="12", Name="Zion", Address="1101 Zion Park Blvd, Springdale, UT 84767", Notes= "Combine with Arches"},
            new Park{Key="13", Name="Glacier", Address="64 Grinnell Dr, West Glacier, MT 59936", Notes= "Do it before glaciers become history"},
            new Park{Key="14", Name="Theodore Roosevelt", Address="208 Scenic Dr, Watford City, ND 58854", Notes= "Plan for 2022"},
            new Park{Key="15", Name="Yellow Stone", Address="P.O. Box 168, Yellowstone National Park, WY 82190", Notes= "It was humbling, do it again!"},
            new Park{Key="16", Name="Isle Royale", Address="800 E Lakeshore Dr, Houghton, MI 49931", Notes= "Have to fly to this one"},
            new Park{Key="17", Name="Rocky Mountain", Address="1000 Us Highway 36, Estes Park, CO 80517", Notes= "Late fall may be good time"},
            new Park{Key="18", Name="Mammoth Cave", Address="1 Mammoth Cave Pkwy, Mammoth Cave, KY 42259", Notes= "Plan for 2024"},
        };

        [HttpGet("[action]")]
        public IEnumerable<Park> Parks()
        {
            return _parks;
        }

        public class Park
        {
            public string Key { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Notes { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}
