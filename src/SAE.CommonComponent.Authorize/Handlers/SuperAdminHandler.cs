using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;

namespace SAE.CommonComponent.Authorize.Handlers
{
    public class SuperAdminHandler : AbstractHandler<SuperAdmin>,
                                  ICommandHandler<SuperAdminCommand.Create>,
                                  ICommandHandler<SuperAdminCommand.Delete>,
                                  ICommandHandler<SuperAdminCommand.List, IEnumerable<string>>


    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IDirector _director;
        private readonly ILogging _logging;

        public SuperAdminHandler(IDocumentStore documentStore,
                                 IStorage storage,
                                 IMediator mediator,
                                 IDirector director,
                                 ILogging<UserRoleHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._director = director;
            this._logging = logging;
        }

        public async Task HandleAsync(SuperAdminCommand.Create command)
        {
            var superAdmin = new SuperAdmin(command);
            await this.AddAsync(superAdmin);
        }

        public async Task HandleAsync(SuperAdminCommand.Delete command)
        {
            await this.DeleteAsync(command.Id);
        }

        public async Task<IEnumerable<string>> HandleAsync(SuperAdminCommand.List command)
        {
            Assert.Build(command.TargetId)
                  .NotNullOrWhiteSpace($"请提供'{nameof(command.TargetId)}'标识!");
            return this._storage.AsQueryable<SuperAdmin>()
                       .Where(s => s.TargetId == command.TargetId)
                       .Select(s=>s.AppId)
                       .ToArray();
        }
    }
}