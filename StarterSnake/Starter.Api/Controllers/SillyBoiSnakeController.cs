//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using Starter.Api.DAL;
using Starter.Api.Requests;
using Starter.Api.Responses;
//using Starter.Core;
using Starter.Core.Solvers;

namespace Starter.Api.Controllers
{
    [ApiController]
    [Route("SillyBoi/Snake")]
    public class SillyBoiSnakeController : ControllerBase
    {
        /// <summary>
        /// This request will be made periodically to retrieve information about your Battlesnake,
        /// including its display options, author, etc.
        /// </summary>
        [HttpGet("")]
        public IActionResult Index()
        {
            var response = new InitResponse
            {
                ApiVersion = "1",
                Author = "Defines Fineout",
                Color = "#EE0077",
                Head = "trans-rights-scarf",
                Tail = "present"
            };

            return Ok(response);
        }


        /// <summary>
        /// Your Battlesnake will receive this request when it has been entered into a new game.
        /// Every game has a unique ID that can be used to allocate resources or data you may need.
        /// Your response to this request will be ignored.
        /// </summary>
        [HttpPost("start")]
        public IActionResult Start(GameStatusRequest gameStatusRequest)
        {
            return Ok();
        }


        /// <summary>
        /// This request will be sent for every turn of the game.
        /// Use the information provided to determine how your
        /// Battlesnake will move on that turn, either up, down, left, or right.
        /// </summary>
        [HttpPost("move")]
        public IActionResult Move(GameStatusRequest gameStatusRequest)
        {
            var solver = new NaiveSolver(gameStatusRequest.Board, gameStatusRequest.You);

            var direction = solver.GetNextDirection();
            var response = new MoveResponse
            {
                Move = direction.ToString().ToLower(),
                Shout = $"I am totally not moving {direction}!"
            };

            //var log = new MoveLog
            //{
            //    Request = gameStatusRequest,
            //    Response = response
            //};

            //try
            //{
            //    MoveLogRepository.AddLog(log);
            //}
            //catch { }

            return Ok(response);
        }

        /// <summary>
        /// Your Battlesnake will receive this request whenever a game it was playing has ended.
        /// Use it to learn how your Battlesnake won or lost and deallocated any server-side resources.
        /// Your response to this request will be ignored.
        /// </summary>
        [HttpPost("end")]
        public IActionResult End(GameStatusRequest gameStatusRequest)
        {
            return Ok();
        }
    }
}