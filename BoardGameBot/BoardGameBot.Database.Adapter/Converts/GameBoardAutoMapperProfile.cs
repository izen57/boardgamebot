using CommonModels = BoardGameBot.Models;
using DBModels = BoardGameBot.Database.PostgreSQL.Models;
using AutoMapper;

namespace BoardGameBot.Database.Adapter.Converts
{
	public class GameBoardAutoMapperProfile: Profile
	{
        public GameBoardAutoMapperProfile()
        {
            CreateMap<DBModels.Game, CommonModels.Game>().ForMember(
              x => x.GameOwners,
              x => x.MapFrom(y => y.GameOwners)
            );
            CreateMap<DBModels.GameOwner, CommonModels.GameOwner>().ForMember(
              x => x.Games,
              x => x.MapFrom(y => y.Games)
            );

            CreateMap<DBModels.Group, CommonModels.Group>().ForMember(
              x => x.Members,
              x => x.MapFrom(y => y.Members)
            ).ForMember(
              x => x.Admins,
              x => x.MapFrom(y => y.Admins)
            ).ForMember(
              x => x.Polls,
              x => x.MapFrom(y => y.Polls)
            );

            CreateMap<DBModels.Poll, CommonModels.Poll>().ForMember(
              x => x.Group,
              x => x.MapFrom(y => y.Group)
            );

            CreateMap<CommonModels.Game, DBModels.Game>().ForMember(
            x => x.GameOwners,
              x => x.MapFrom(y => y.GameOwners)
            );
            CreateMap<CommonModels.GameOwner, DBModels.GameOwner>().ForMember(
              x => x.Games,
              x => x.MapFrom(y => y.Games)
            );

            CreateMap<CommonModels.Group, DBModels.Group>().ForMember(
              x => x.Members,
              x => x.MapFrom(y => y.Members)
            ).ForMember(
              x => x.Admins,
              x => x.MapFrom(y => y.Admins)
            ).ForMember(
              x => x.Polls,
              x => x.MapFrom(y => y.Polls)
            );

            CreateMap<CommonModels.Poll, DBModels.Poll>().ForMember(
              x => x.Group,
              x => x.MapFrom(y => y.Group)
            );
        }
    }
}
