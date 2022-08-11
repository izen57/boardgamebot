using Models = BoardGameBot.Models;

using DBModels = BoardGameBot.Database.PostgreSQL.Models;
using AutoMapper;

namespace BoardGameBot.Database.Adapter.Converts
{
	public class GameBoardAutoMapperProfile: Profile
	{
		public GameBoardAutoMapperProfile()
		{
			CreateMap<DBModels.Game, Models.Game>().ForMember(
				x => x.GameOwners,
				x => x.MapFrom(y => y.GameOwners)
			);
			CreateMap<DBModels.GameOwner, Models.GameOwner>().ForMember(
				x => x.Games,
				x => x.MapFrom(y => y.Games)
			);
		}
	}
}
