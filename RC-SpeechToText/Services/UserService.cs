using Microsoft.EntityFrameworkCore;
using RC_SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services
{
	public class UserService
	{
		private readonly SearchAVContext _context;

		public UserService(SearchAVContext context)
		{
			_context = context;
		}

		public async Task<User> CreateUser(User user)
		{
			if (!await _context.User.AnyAsync(u => u.Email == user.Email))
			{
				// Store in DB
				await _context.User.AddAsync(user);
				await _context.SaveChangesAsync();

				return user;
			}

			return user;
		}

		public async Task<List<User>> GetAllUsers()
		{
			return await _context.User.ToListAsync();
		}

		public async Task<User> GetUserName(int id)
		{
			return await _context.User.FindAsync(id);
		}

		public async Task<User> GetUserByEmail(string email)
		{
			return await _context.User.Where(u => u.Email == email).FirstOrDefaultAsync();
		}
	}
}
