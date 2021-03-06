﻿using System;
using System.Threading.Tasks;
using TastyDomainDriven.AggregateService;

namespace TastyDomainDriven.AsyncImpl
{
    public abstract class AsyncCommandDispatcher<TAggregateRoot> : IAsyncCommandDispatcher where TAggregateRoot : IAggregate, new()
    {
        private readonly IEventStoreAsync eventStore;

        protected AsyncCommandDispatcher(IEventStoreAsync eventStore)
        {
            this.eventStore = eventStore;
        }

        protected async Task Update<TIdent>(TIdent id, Action<TAggregateRoot> execute) where TIdent : IIdentity
        {
            await new UpdateAggregateAsync<TAggregateRoot>().Execute(this.eventStore, id, execute);
        }

        /// <summary>
        /// Create a aggreagate from another existing root. Only changes to the new aggregate is saved. 
        /// </summary>
        /// <typeparam name="TIdent">id type of created aggregate</typeparam>
        /// <typeparam name="TResult">Aggregate to save changes on</typeparam>
        /// <typeparam name="TCreateFromId">id to create from</typeparam>
        /// <param name="id">existing aggregate id</param>
        /// <param name="execute">invoke logic</param>
        /// <param name="createId">id to save changes to</param>
        protected async Task Create<TIdent, TResult, TCreateFromId>(TCreateFromId id, Func<TResult, TAggregateRoot> execute, TIdent createId)
            where TIdent : IIdentity
            where TCreateFromId : IIdentity
            where TResult : IAggregate, new()
        {
            // Load event stream from the store
            await new CreateAggregateByUpdateAsync<TAggregateRoot>().Execute(this.eventStore, id, execute, createId);
        }

        public abstract Task Dispatch(ICommand command);
    }
}