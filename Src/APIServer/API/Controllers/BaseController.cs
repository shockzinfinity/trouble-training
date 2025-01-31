﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace APIServer.Controllers
{

  [ApiController]
  [Route("api/[controller]/[action]")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public abstract class BaseController : ControllerBase
  {
    private IMediator? _mediator;
    protected IMediator? Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

  }
}
