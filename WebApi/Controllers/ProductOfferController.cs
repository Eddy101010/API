﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hub;

namespace WebApi.Controllers
{
    public class ProductOfferController : BaseController
    {
        private IHubContext<MessageHub, IMessageHubClient> messageHub;
        public ProductOfferController(IHubContext<MessageHub, IMessageHubClient> _messageHub)
        {
           messageHub = _messageHub;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("productoffers")]
        
        public string Get()
        {
            List<string> offers = new List<string>();
            offers.Add("20% Off on Property 12");
            offers.Add("15% Off on Property 14");
            offers.Add("30% Off on Property 16");
            messageHub.Clients.All.SendOffersToUsers(offers);
            return "Offers sent succesfully to all users!";
        }

    }
}
