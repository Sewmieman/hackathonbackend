using System.ComponentModel.DataAnnotations;
using FixMyCampus.Api.Models;

namespace FixMyCampus.Api.DTOs;

public record RegisterDto
(
    [Required]
    string FullName,

    [Required]
    [EmailAddress]
    string Email,

    [Required]
    [MinLength(6)]
    string Password,

    [Required]
    UserRole Role
);

public record LoginDto
(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string Password,

    [Required]
    UserRole Role
);

public record LoginResponseDto
(
    int Id,
    string FullName,
    string Email,
    UserRole Role,
    string Token
);