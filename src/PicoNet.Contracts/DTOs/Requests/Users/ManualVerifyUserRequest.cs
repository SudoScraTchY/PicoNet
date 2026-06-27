using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Users;

public record ManualVerifyUserRequest
{
    public bool SetEmailConfirmed { get; set; } = true;
    public bool SetPhoneConfirmed { get; set; } = false;
}

