using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SugarChat.Core.Utils
{
    public interface ITableUtil
    {
        IQueryable<TableJoinDto> GetQuery(string userId, IEnumerable<string> filterGroupIds, int groupType, bool IsJoinMessage);
        string GetWhereByGroupCustomPropery(SearchGroupByGroupCustomPropertiesDto groupByGroupCustomProperties, string keyName = "Key", string valueName = "Value");
        string GetWhereByMessage(Dictionary<string, string> searchParms, bool isExactSearch, string keyName = "Key", string valueName = "Value");
    }

    public class TableUtil : ITableUtil
    {
        private readonly IRepository _repository;

        public TableUtil(IRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<TableJoinDto> GetQuery(string userId,
            IEnumerable<string> filterGroupIds,
            int groupType,
            bool IsJoinMessage)
        {
            IQueryable<TableJoinDto> query = null;
            if (filterGroupIds != null && filterGroupIds.Any())
            {
                if (IsJoinMessage)
                {
                    query = from a in _repository.Query<GroupUser>()
                            join b in _repository.Query<Group>() on a.GroupId equals b.Id
                            join c in _repository.Query<GroupCustomProperty>() on b.Id equals c.GroupId
                            join d in _repository.Query<Domain.Message>() on b.Id equals d.GroupId
                            join e in _repository.Query<MessageCustomProperty>() on d.Id equals e.MessageId
                            where a.UserId == userId && b.Type == groupType && filterGroupIds.Contains(a.GroupId)
                            select new TableJoinDto
                            {
                                GroupId = a.GroupId,
                                UnreadCount = a.UnreadCount,
                                LastSentTime = b.LastSentTime,
                                GroupKey = c.Key,
                                GroupValue = c.Value,
                                Content = d.Content,
                                MessageKey = e.Key,
                                MessageValue = e.Value
                            };
                }
                else
                {
                    query = from a in _repository.Query<GroupUser>()
                            join b in _repository.Query<Group>() on a.GroupId equals b.Id
                            join c in _repository.Query<GroupCustomProperty>() on b.Id equals c.GroupId
                            where a.UserId == userId && b.Type == groupType && filterGroupIds.Contains(a.GroupId)
                            select new TableJoinDto
                            {
                                GroupId = a.GroupId,
                                UnreadCount = a.UnreadCount,
                                LastSentTime = b.LastSentTime,
                                GroupKey = c.Key,
                                GroupValue = c.Value
                            };
                }
            }
            else
            {
                if (IsJoinMessage)
                {
                    query = from a in _repository.Query<GroupUser>()
                            join b in _repository.Query<Group>() on a.GroupId equals b.Id
                            join c in _repository.Query<GroupCustomProperty>() on b.Id equals c.GroupId
                            join d in _repository.Query<Domain.Message>() on b.Id equals d.GroupId
                            join e in _repository.Query<MessageCustomProperty>() on d.Id equals e.MessageId
                            where a.UserId == userId && b.Type == groupType
                            select new TableJoinDto
                            {
                                GroupId = a.GroupId,
                                UnreadCount = a.UnreadCount,
                                LastSentTime = b.LastSentTime,
                                GroupKey = c.Key,
                                GroupValue = c.Value,
                                Content = d.Content,
                                MessageKey = e.Key,
                                MessageValue = e.Value
                            };
                }
                else
                {
                    query = from a in _repository.Query<GroupUser>()
                            join b in _repository.Query<Group>() on a.GroupId equals b.Id
                            join c in _repository.Query<GroupCustomProperty>() on b.Id equals c.GroupId
                            where a.UserId == userId && b.Type == groupType
                            select new TableJoinDto
                            {
                                GroupId = a.GroupId,
                                UnreadCount = a.UnreadCount,
                                LastSentTime = b.LastSentTime,
                                GroupKey = c.Key,
                                GroupValue = c.Value
                            };
                }
            }
            return query;
        }

        public string GetWhereByGroupCustomPropery(SearchGroupByGroupCustomPropertiesDto groupByGroupCustomProperties, string keyName = "Key", string valueName = "Value")
        {
            var sb = new StringBuilder();
            if (groupByGroupCustomProperties != null && groupByGroupCustomProperties.GroupCustomProperties != null)
            {
                foreach (var dic in groupByGroupCustomProperties.GroupCustomProperties)
                {
                    foreach (var value in dic.Value)
                    {
                        var value1 = value.Replace("\\", "\\\\");
                        var key1 = dic.Key.Replace("\\", "\\\\");
                        if (value1.Contains(","))
                        {
                            var values = value1.Split(',');
                            foreach (var value2 in values)
                            {
                                if (groupByGroupCustomProperties.IsExactSearch)
                                    sb.Append($" || ({keyName}==\"{key1}\" && {valueName}==\"{value2}\")");
                                else
                                    sb.Append($" || ({keyName}==\"{key1}\" && {valueName}.Contains(\"{value2}\"))");
                            }
                        }
                        else
                        {
                            if (groupByGroupCustomProperties.IsExactSearch)
                                sb.Append($" || ({keyName}==\"{key1}\" && {valueName}==\"{value1}\")");
                            else
                                sb.Append($" || ({keyName}==\"{key1}\" && {valueName}.Contains(\"{value1}\"))");
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public string GetWhereByMessage(Dictionary<string, string> searchParms, bool isExactSearch, string keyName = "Key", string valueName = "Value")
        {
            var sb = new StringBuilder();
            if (searchParms != null && searchParms.Any())
            {
                var contentSearchParms = searchParms.Where(x => x.Key == Message.Constant.Content);
                var customPropertySearchParms = searchParms.Where(x => x.Key != Message.Constant.Content);
                if (contentSearchParms.Any())
                {
                    foreach (var contentSearchParm in contentSearchParms)
                    {
                        var value = contentSearchParm.Value.Replace("\\", "\\\\");
                        if (isExactSearch)
                        {
                            sb.Append($" || {Message.Constant.Content}==\"{value}\"");
                        }
                        else
                        {
                            sb.Append($" || {Message.Constant.Content}.Contains(\"{value}\")");
                        }
                    }
                }
                if (customPropertySearchParms.Any())
                {
                    foreach (var customPropertySearchParm in customPropertySearchParms)
                    {
                        var values = customPropertySearchParm.Value.Split(',');
                        foreach (var value in values)
                        {
                            var _value = customPropertySearchParm.Value.Replace("\\", "\\\\");
                            if (isExactSearch)
                            {
                                var _sb = $"{keyName}==\"{customPropertySearchParm.Key}\" && {valueName} == \"{_value}\"";
                                sb.Append($" || ({_sb})");
                            }
                            else
                            {
                                var _sb = $"{keyName}.Contains(\"{customPropertySearchParm.Key}\") && {valueName}.Contains(\"{_value}\")";
                                sb.Append($" || ({_sb})");
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }
}
