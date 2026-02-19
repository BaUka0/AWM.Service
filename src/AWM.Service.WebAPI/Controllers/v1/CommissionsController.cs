namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Defense.Commissions.Commands.AddCommissionMember;
using AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;
using AWM.Service.Application.Features.Defense.Commissions.Commands.RemoveCommissionMember;
using AWM.Service.Application.Features.Defense.Commissions.Commands.UpdateCommission;
using AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissionById;
using AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissionsByDepartment;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using AWM.Service.WebAPI.Common.Contracts.Responses.Defense;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing Defense Commissions (PreDefense and GAK).
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/commissions")]
[Produces("application/json")]
public class CommissionsController : BaseController
{
    private readonly ISender _sender;

    public CommissionsController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get commissions for a department in a given academic year.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic year ID</param>
    /// <returns>List of commissions</returns>
    [HttpGet]
    [RequireDepartmentPermission(Permission.Commissions_View)]
    [ProducesResponseType(typeof(IReadOnlyList<CommissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByDepartment(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId)
    {
        var query = new GetCommissionsByDepartmentQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var response = result.Value
            .Select(c => new CommissionResponse
            {
                Id = c.Id,
                DepartmentId = c.DepartmentId,
                AcademicYearId = c.AcademicYearId,
                CommissionType = c.CommissionType,
                Name = c.Name,
                PreDefenseNumber = c.PreDefenseNumber,
                MemberCount = c.MemberCount,
                CreatedAt = c.CreatedAt
            })
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Get a specific commission by ID with full details including members.
    /// </summary>
    /// <param name="id">Commission ID</param>
    /// <returns>Commission with member list</returns>
    [HttpGet("{id:int}")]
    [RequireDepartmentPermission(Permission.Commissions_View)]
    [ProducesResponseType(typeof(CommissionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetCommissionByIdQuery { CommissionId = id };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var dto = result.Value;
        var response = new CommissionDetailResponse
        {
            Id = dto.Id,
            DepartmentId = dto.DepartmentId,
            AcademicYearId = dto.AcademicYearId,
            CommissionType = dto.CommissionType,
            Name = dto.Name,
            PreDefenseNumber = dto.PreDefenseNumber,
            CreatedAt = dto.CreatedAt,
            CreatedBy = dto.CreatedBy,
            LastModifiedAt = dto.LastModifiedAt,
            LastModifiedBy = dto.LastModifiedBy,
            Members = dto.Members
                .Select(m => new CommissionMemberResponse
                {
                    Id = m.Id,
                    CommissionId = m.CommissionId,
                    UserId = m.UserId,
                    RoleInCommission = m.RoleInCommission,
                    CreatedAt = m.CreatedAt
                })
                .ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Create a new defense commission.
    /// </summary>
    /// <param name="request">Create commission request</param>
    /// <returns>Created commission ID</returns>
    [HttpPost]
    [RequireDepartmentPermission(Permission.Commissions_Manage)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCommissionRequest request)
    {
        var command = new CreateCommissionCommand
        {
            DepartmentId = request.DepartmentId,
            AcademicYearId = request.AcademicYearId,
            CommissionType = request.CommissionType,
            Name = request.Name,
            PreDefenseNumber = request.PreDefenseNumber
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Update a commission's name.
    /// </summary>
    /// <param name="id">Commission ID</param>
    /// <param name="request">Update request</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id:int}")]
    [RequireDepartmentPermission(Permission.Commissions_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCommissionRequest request)
    {
        var command = new UpdateCommissionCommand
        {
            CommissionId = id,
            Name = request.Name
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Add a member to a commission.
    /// </summary>
    /// <param name="id">Commission ID</param>
    /// <param name="request">Add member request</param>
    /// <returns>Created member ID</returns>
    [HttpPost("{id:int}/members")]
    [RequireDepartmentPermission(Permission.Commissions_ManageMembers)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddMember(int id, [FromBody] AddCommissionMemberRequest request)
    {
        var command = new AddCommissionMemberCommand
        {
            CommissionId = id,
            UserId = request.UserId,
            RoleInCommission = request.RoleInCommission
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id }, result.Value);
    }

    /// <summary>
    /// Remove a member from a commission.
    /// </summary>
    /// <param name="id">Commission ID</param>
    /// <param name="memberId">Member record ID to remove</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}/members/{memberId:int}")]
    [RequireDepartmentPermission(Permission.Commissions_ManageMembers)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveMember(int id, int memberId)
    {
        var command = new RemoveCommissionMemberCommand
        {
            CommissionId = id,
            MemberId = memberId
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }
}
