using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FeedService.Domain.Aggregates;
using FeedService.Domain.Commands.PostCommands;
using FeedService.Domain.Commands.PostCommands.Comment;
using FeedService.Domain.Repositories;
using FeedService.Domain.States;
using MediatR;

namespace FeedService.Domain.CommandHandlers
{
    public class PostCommandHandler:
        IRequestHandler<CreatePost>,
        IRequestHandler<DeletePost>,
        IRequestHandler<AddComment>,
        IRequestHandler<RemoveComment>
    {
        private readonly IPostRepository _postRepository;

        public PostCommandHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public Task<Unit> Handle(CreatePost request, CancellationToken cancellationToken)
        {
            _postRepository.Save(new PostAggregate(request));
            return Unit.Task;
        }

        public Task<Unit> Handle(DeletePost request, CancellationToken cancellationToken)
        {
            var aggregate = _postRepository.GetAggregateById(request.Id);

            if (aggregate.GetState().Person.UserId != request.UserId)
            {
                throw new Exception("Não é possivel deletar um post de outro usuario");
            }
                
            aggregate.Delete();
            _postRepository.Save(aggregate);
            return Unit.Task;
        }

        public Task<Unit> Handle(AddComment request, CancellationToken cancellationToken)
        {
            var aggregate = _postRepository.GetAggregateById(request.PostId);
            
            aggregate.AddComment(request);
            
            _postRepository.Save(aggregate);
            return Unit.Task;
        }

        public Task<Unit> Handle(RemoveComment request, CancellationToken cancellationToken)
        {
            var comment = _postRepository.GetCommentById(request.Id);
            
            var aggregate = _postRepository.GetAggregateById(comment.Post.PostId);
            
            if (aggregate.GetState().Comments.FirstOrDefault(x => x.CommentId == request.Id)?.Person.UserId 
                != request.UserId)
            {
                throw new Exception("Não é possivel deletar um post de outro usuario");
            }
            
            aggregate.DeleteComment(request);
            
            _postRepository.Save(aggregate);
            return Unit.Task;
        }
    }
}