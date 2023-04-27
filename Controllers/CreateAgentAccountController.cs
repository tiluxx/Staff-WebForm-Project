using Agent_WebForm_Prodject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agent_WebForm_Prodject.Controllers
{
    public class CreateAgentAccountController : Controller
    {
        // GET: CreateAgentAccount
        public ActionResult Index()
        {
            Agent agent = new Agent();
            List<Agent> res = agent.SelectAgentQuery();
            ViewBag.AgentList = res;
            return View();
        }

        // GET: CreateAgentAccount/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: CreateAgentAccount/ViewAgentAccount
        public ActionResult ViewAgentAccount(string agentId)
        {
            ViewBag.Data = agentId;
            return View();
        }

        private string GenerateAccountUsername(string agentName)
        {
            Random res = new Random();
            string str = "0123456789";
            int size = 4;

            string result = "";
            for (int i = 0; i < size; i++)
            {
                int x = res.Next(str.Length);
                result += str[x];
            }

            string agentNameStandardlized = agentName.Replace(" ", "").ToUpper();
            return agentNameStandardlized + result;
        }

        private string GenerateAccountPassword(string agentID)
        {
            Random res = new Random();
            string str = "abcdefghijklmnopqrstuvwxyz0123456789";
            int size = 5;

            string result = "";
            for (int i = 0; i < size; i++)
            {
                int x = res.Next(str.Length);
                result += str[x];
            }

            return agentID + "_" + result;
        }

        [HttpPost]
        public ActionResult CreateNewAgent()
        {
            string agentId = Request.Form["AgentId"];
            string agentName = Request.Form["AgentName"];
            string agentPhone = Request.Form["AgentPhone"];
            string agentEmail = Request.Form["AgentEmail"];
            string agentAddress = Request.Form["AgentAddress"];
            Agent agent = new Agent();
            agent.AddAgentQuery(agentId, agentName, agentEmail, agentPhone, agentAddress);

            string username = GenerateAccountUsername(agentName);
            string password = GenerateAccountPassword(agentId);
            AgentAccount agentAccount = new AgentAccount();
            agentAccount.AddAgentAcQuery(agentId, username, password, false);

            ViewBag.Message = "Create new agent and agent account successfully";
            return View("Result");
        }
    }
}