using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Users.DataTransfer;
using ChatBackend.Users.Domain;
using ChatBackend.Users.Domain.Operations;
using ChatBackend.Users.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ChatBackend.Users.Controllers
{
    /// <summary>
    /// Контроллер пользователей чата
    /// </summary>
    [ApiController]
    [Route("user")]
    [Authorize]
    public sealed class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="userService"></param>
        public UserController(
            ILogger<UserController> logger,
            IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Возвращает список всех пользователей чата
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetAllAsync(CancellationToken cancellationToken) =>
            Ok(await _userService.GetAsync(cancellationToken));

        /// <summary>
        /// Возвращает пользователя в текущей сессии
        /// </summary>
        /// <param name="cancellationToken"></param>
        [HttpGet("current")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetCurrentAsync(CancellationToken cancellationToken)
        {
            var guid = Guid.Parse(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            return Ok(await _userService.GetByGuidAsync(guid, cancellationToken));
        }

        /// <summary>
        /// Возвращает пользователя с указанным Guid
        /// </summary>
        /// <param name="userGuid">Guid пользователя</param>
        /// <param name="cancellationToken"></param>
        [HttpGet("by-guid/{userGuid:guid}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetByGuidAsync(Guid userGuid, CancellationToken cancellationToken)
        {
            if (!await _userService.CheckIfGuidExistsAsync(userGuid, cancellationToken))
                return NotFound(userGuid);
            return Ok(await _userService.GetByGuidAsync(userGuid, cancellationToken));
        }

        /// <summary>
        /// Аутентифицирует пользователя в чате
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="cancellationToken"></param>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<ValidationFailure>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> LogInAsync(
            string login, 
            string password,
            CancellationToken cancellationToken)
        {
            var userInfo = await _userService.LogInAsync(login, password, cancellationToken);
            
            if (userInfo == default)
                return BadRequest();
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.Guid.ToString()),
                new Claim(ClaimTypes.Name, userInfo.Login!),
            }.Union(
                userInfo.Roles!.Select(role => new Claim(
                    ClaimTypes.Role, role.NormalizedName.ToString())))
                .ToArray();

            var identity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: DateTime.UtcNow,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(encodedJwt);
        }
        
        // /// <summary>
        // /// Забывает пользователя
        // /// </summary>
        // /// <param name="cancellationToken"></param>
        // [HttpPost("logout")]
        // [AllowAnonymous]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public Task LogOutAsync(CancellationToken cancellationToken) => 
        //     HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        /// <summary>
        /// Создаёт нового пользователя чата
        /// </summary>
        /// <param name="createUser">Данные нового пользователя</param>
        /// <param name="cancellationToken"></param>
        [HttpPost("create")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IEnumerable<ValidationFailure>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<UserDto>> CreateUserAsync(
            CreateUser createUser,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.CreateAsync(createUser, cancellationToken);
                return Created(user.Guid.ToString(), createUser);
            }
            catch (ValidationException validationException)
            {
                return BadRequest(validationException.Errors);
            }
        }
        
        /// <summary>
        /// Создаёт нового посыльного
        /// </summary>
        /// <param name="createMessenger">Данные нового посыльного</param>
        /// <param name="cancellationToken"></param>
        [HttpPost("create-messenger")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IEnumerable<ValidationFailure>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<UserDto>> CreateMessengerAsync(
            CreateMessenger createMessenger,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.CreateAsync(createMessenger.ToCreateUser(), cancellationToken);
                return Created(user.Guid.ToString(), user);
            }
            catch (ValidationException validationException)
            {
                return BadRequest(validationException.Errors);
            }
        }

        /// <summary>
        /// Возвращает все доступные роли пользователей чата
        /// </summary>
        [HttpGet("roles/all")]
        [ProducesResponseType(typeof(UserRole[]), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public ActionResult<UserRole[]> GetRoles() =>
            Ok(UserRole.GetAll());
    }
}