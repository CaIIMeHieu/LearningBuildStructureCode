using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Contract.Abstractions.Message;

public interface IDomainHandler<TEvent> : INotificationHandler<TEvent> where TEvent : IDomain
{

}
