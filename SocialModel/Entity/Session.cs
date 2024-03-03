﻿using System.Linq.Expressions;

namespace SocialModel.Entity;

/// <summary>
/// An entity describing an ongoing user session or login request.
/// </summary>
public class Session : IdKeyEntity
{
    /// <summary>
    /// The entire primary key, <see cref="int">int</see> identity column.
    /// </summary>
    [IdKey]
    public int SessionId { get; init; }

    /// <summary>
    /// The request ID generated by the login request procedure.
    /// </summary>
    /// <value>A Guid representation of the request ID.</value>
    public required Guid RequestId { get; set; }

    /// <summary>
    /// A SHA-256 hash created from the generated secret.
    /// </summary>
    /// <value>The hash as a byte array. Will always be 256 in size.</value>
    [MinLength(256)]
    [MaxLength(256)]
    public required byte[] RequestSecretHash { get; set; }

    /// <summary>
    /// Additional entropy used in generating the hash from the secret for this request.
    /// </summary>
    /// <value>A Guid representation of the entropy value.</value>
    public required Guid RequestSecretEntropy { get; set; }

    /// <summary>
    /// The impersonating token that this session provides. This is NOT to be treated as a valid token if <see cref="Validated"/> is false.
    /// </summary>
    /// <value>A Guid whose string representation is treated as an authenticating bearer token.</value>
    public required Guid ImpersonateToken { get; set; }

    /// <summary>
    /// A column describing if the login procedure for this session was completed, and the generated token is valid authentication.
    /// </summary>
    /// <value><see langword="true"/> if the login procedure was completed, <see langword="false"/> if it is considered ongoing.</value>
    public required bool Validated { get; set; }

    /// <summary>
    /// The user that this session authenticates, and presuambly the person that started the login request.
    /// </summary>
    /// <value>The <see cref="User"/> relevant to this session.</value>
    public required User ProcessOwner { get; init; }
}