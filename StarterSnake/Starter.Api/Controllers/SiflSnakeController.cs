using Microsoft.AspNetCore.Mvc;
using Starter.Api.Requests;
using Starter.Api.Responses;
using Starter.Core.Enumerations;
using System.Threading.Tasks;

namespace Starter.Api.Controllers
{
    [ApiController]
    [Route("Sifl/Snake")]
    public class SiflSnakeController : ControllerBase
    {
        public const int MoveDelayMilliseconds = 350;
        public static Direction JoystickState = Direction.None;

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
                Color = "#cccccc",
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
        public async Task<IActionResult> Move(GameStatusRequest gameStatusRequest)
        {
            // Wait a moment for the slowly human...
            await Task.Delay(MoveDelayMilliseconds);

            // Move based on latest joystick info
            var response = new MoveResponse
            {
                Move = JoystickState.ToString().ToLower(), //TODO: Direction extension ForResponse
                Shout = $"Human says I has to go {JoystickState}..."
            };

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

        /// <summary>
        /// Post human-supplied direction input to the snake.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        [HttpPost("input")]
        public IActionResult Input(Direction direction)
        {
            JoystickState = direction;
            return Ok();
        }
    }
}