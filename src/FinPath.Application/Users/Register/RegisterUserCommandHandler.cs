using FinPath.Application.Common.Exceptions;
using FinPath.Application.Common.Interfaces;
using FinPath.Domain.Users;
using MediatR;

namespace FinPath.Application.Users.Register
{
    public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToUpperInvariant();

            var userExists = await _userRepository.ExistsByNormalizedEmailAsync(normalizedEmail, cancellationToken);

            if (userExists)
                throw new ConflictException("Пользователь с таким адресом электронной почты уже зарегистрирован.");

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var user = User.Create(
                request.Email, 
                request.DisplayName,
                passwordHash);

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterUserResult(user.Id, user.Email, user.DisplayName);
        }
    }
}
