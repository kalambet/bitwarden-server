﻿using Bit.Api.Models.Response;
using Bit.Api.SecretsManager.Models.Response;
using Bit.Core.Context;
using Bit.Core.Exceptions;
using Bit.Core.SecretsManager.Commands.Secrets.Interfaces;
using Bit.Core.SecretsManager.Commands.Trash.Interfaces;
using Bit.Core.SecretsManager.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bit.Api.SecretsManager.Controllers;

[SecretsManager]
[Authorize("secrets")]
public class TrashController : Controller
{
    private readonly ICurrentContext _currentContext;
    private readonly ISecretRepository _secretRepository;
    private readonly ICreateSecretCommand _createSecretCommand;
    private readonly IUpdateSecretCommand _updateSecretCommand;
    private readonly IDeleteSecretCommand _deleteSecretCommand;
    private readonly IEmptyTrashCommand _emptyTrashCommand;

    public TrashController(
        ICurrentContext currentContext,
        ISecretRepository secretRepository,
        ICreateSecretCommand createSecretCommand,
        IUpdateSecretCommand updateSecretCommand,
        IDeleteSecretCommand deleteSecretCommand,
        IEmptyTrashCommand emptyTrashCommand)
    {
        _currentContext = currentContext;
        _secretRepository = secretRepository;
        _createSecretCommand = createSecretCommand;
        _updateSecretCommand = updateSecretCommand;
        _deleteSecretCommand = deleteSecretCommand;
        _emptyTrashCommand = emptyTrashCommand;
    }

    [HttpGet("secrets/{organizationId}/trash")]
    public async Task<SecretWithProjectsListResponseModel> ListByOrganizationAsync([FromRoute] Guid organizationId)
    {
        if (!_currentContext.AccessSecretsManager(organizationId))
        {
            throw new NotFoundException();
        }
        if (!await _currentContext.OrganizationAdmin(organizationId))
        {
            throw new UnauthorizedAccessException();
        }

        var secrets = await _secretRepository.GetManyByOrganizationIdInTrashAsync(organizationId);
        return new SecretWithProjectsListResponseModel(secrets);
    }

    [HttpPost("secrets/{organizationId}/trash/empty")]
    public async Task<ListResponseModel<BulkDeleteResponseModel>> EmptyTrash([FromRoute] Guid organizationId, [FromBody] List<Guid> ids)
    {
        if (!_currentContext.AccessSecretsManager(organizationId))
        {
            throw new NotFoundException();
        }
        if (!await _currentContext.OrganizationAdmin(organizationId))
        {
            throw new UnauthorizedAccessException();
        }

        var results = await _emptyTrashCommand.EmptyTrash(organizationId, ids);
        var responses = results.Select(r => new BulkDeleteResponseModel(r.Item1.Id, r.Item2));
        return new ListResponseModel<BulkDeleteResponseModel>(responses);
    }
}