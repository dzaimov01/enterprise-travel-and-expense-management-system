using AutoMapper;
using enterprise_travel_and_expense_management_system.Models.DTOs;
using enterprise_travel_and_expense_management_system.Models.Entities;

namespace enterprise_travel_and_expense_management_system.Infrastructure.Mapping;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs.
/// Centralizes all entity-to-DTO and DTO-to-entity mappings.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the MappingProfile class and configures all mappings.
    /// </summary>
    public MappingProfile()
    {
        // TravelRequest mappings
        CreateMap<TravelRequest, TravelRequestDto>();
        CreateMap<TravelRequestDto, TravelRequest>();

        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();

        // Expense mappings
        CreateMap<Expense, ExpenseDto>();
        CreateMap<ExpenseDto, Expense>();
    }
}
