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
        IQueryable<TableJoinDto> GetQuery(string userId, IEnumerable<string> filterGroupIds, int groupType, bool IsJoinGroupCustomProperty, bool IsJoinMessage);
        //string GetWhereByGroupCustomPropery(SearchGroupByGroupCustomPropertiesDto groupByGroupCustomProperties, string keyName = "Key", string valueName = "Value");
        string GetWhereByMessage(Dictionary<string, string> searchParms, bool isExactSearch, string keyName = "Key", string valueName = "Value");
        string GetWhere(string userId,
            IEnumerable<string> filterGroupIds,
            int groupType,
            IEnumerable<SearchParamDto> searchParams);
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
            bool IsJoinGroupCustomProperty,
            bool IsJoinMessage)
        {
            IQueryable<TableJoinDto> query = null;
            if (filterGroupIds != null && filterGroupIds.Any())
            {
                if (!IsJoinGroupCustomProperty && !IsJoinMessage)
                {
                    query = from a in _repository.Query<GroupUser>()
                            join b in _repository.Query<Group>() on a.GroupId equals b.Id
                            where a.UserId == userId && b.Type == groupType && filterGroupIds.Contains(a.GroupId)
                            select new TableJoinDto
                            {
                                GroupId = a.GroupId,
                                UnreadCount = a.UnreadCount,
                                LastSentTime = b.LastSentTime
                            };
                }
                if (IsJoinGroupCustomProperty && IsJoinMessage)
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
                if (IsJoinGroupCustomProperty && !IsJoinMessage)
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
                if (!IsJoinGroupCustomProperty && IsJoinMessage)
                {
                    query = from a in _repository.Query<GroupUser>()
                            join b in _repository.Query<Group>() on a.GroupId equals b.Id
                            join d in _repository.Query<Domain.Message>() on b.Id equals d.GroupId
                            join e in _repository.Query<MessageCustomProperty>() on d.Id equals e.MessageId
                            where a.UserId == userId && b.Type == groupType && filterGroupIds.Contains(a.GroupId)
                            select new TableJoinDto
                            {
                                GroupId = a.GroupId,
                                UnreadCount = a.UnreadCount,
                                LastSentTime = b.LastSentTime,
                                Content = d.Content,
                                MessageKey = e.Key,
                                MessageValue = e.Value
                            };
                }
            }
            else
            {
                if (!IsJoinGroupCustomProperty && !IsJoinMessage)
                {
                    query = from a in _repository.Query<GroupUser>()
                            join b in _repository.Query<Group>() on a.GroupId equals b.Id
                            where a.UserId == userId && b.Type == groupType
                            select new TableJoinDto
                            {
                                GroupId = a.GroupId,
                                UnreadCount = a.UnreadCount,
                                LastSentTime = b.LastSentTime
                            };
                }
                if (IsJoinGroupCustomProperty && IsJoinMessage)
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
                if (IsJoinGroupCustomProperty && !IsJoinMessage)
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
                if (!IsJoinGroupCustomProperty && IsJoinMessage)
                {
                    query = from a in _repository.Query<GroupUser>()
                            join b in _repository.Query<Group>() on a.GroupId equals b.Id
                            join d in _repository.Query<Domain.Message>() on b.Id equals d.GroupId
                            join e in _repository.Query<MessageCustomProperty>() on d.Id equals e.MessageId
                            where a.UserId == userId && b.Type == groupType
                            select new TableJoinDto
                            {
                                GroupId = a.GroupId,
                                UnreadCount = a.UnreadCount,
                                LastSentTime = b.LastSentTime,
                                Content = d.Content,
                                MessageKey = e.Key,
                                MessageValue = e.Value
                            };
                }
            }
            return query;
        }

        //public string GetWhereByGroupCustomPropery(SearchGroupByGroupCustomPropertiesDto groupByGroupCustomProperties, string keyName = "Key", string valueName = "Value")
        //{
        //    var sb = new StringBuilder();
        //    if (groupByGroupCustomProperties != null && groupByGroupCustomProperties.GroupCustomProperties != null)
        //    {
        //        foreach (var dic in groupByGroupCustomProperties.GroupCustomProperties)
        //        {
        //            foreach (var value in dic.Value)
        //            {
        //                var value1 = value.Replace("\\", "\\\\");
        //                var key1 = dic.Key.Replace("\\", "\\\\");
        //                if (value1.Contains(","))
        //                {
        //                    var values = value1.Split(',');
        //                    foreach (var value2 in values)
        //                    {
        //                        if (groupByGroupCustomProperties.IsExactSearch)
        //                            sb.Append($" || ({keyName}==\"{key1}\" && {valueName}==\"{value2}\")");
        //                        else
        //                            sb.Append($" || ({keyName}==\"{key1}\" && {valueName}.Contains(\"{value2}\"))");
        //                    }
        //                }
        //                else
        //                {
        //                    if (groupByGroupCustomProperties.IsExactSearch)
        //                        sb.Append($" || ({keyName}==\"{key1}\" && {valueName}==\"{value1}\")");
        //                    else
        //                        sb.Append($" || ({keyName}==\"{key1}\" && {valueName}.Contains(\"{value1}\"))");
        //                }
        //            }
        //        }
        //    }
        //    return sb.ToString();
        //}

        public string GetWhere(string userId,
            IEnumerable<string> filterGroupIds,
            int groupType,
            IEnumerable<SearchParamDto> searchParams)
        {
            if (searchParams == null || !searchParams.Any())
                searchParams = new List<SearchParamDto>();

            var search = new StringBuilder();
            foreach (var searchParam in searchParams)
            {
                List<string> searchs = new List<string>();
                foreach (var searchParamDetail in searchParam.SearchParamDetails)
                {
                    var values = searchParamDetail.Value.Split(",");
                    foreach (var value in values)
                    {
                        var _value = value.Trim();
                        if (searchParamDetail.ValueType == Message.Dtos.ValueType.String)
                            _value = $@"""{_value}""";

                        switch (searchParamDetail.ConditionCondition)
                        {
                            case Condition.Equal:
                                searchs.Add($@"CustomProperties.{searchParamDetail.Key}=={_value}");
                                break;
                            case Condition.Unequal:
                                searchs.Add($@"CustomProperties.{searchParamDetail.Key}!={_value}");
                                break;
                            case Condition.GreaterThan:
                                searchs.Add($@"CustomProperties.{searchParamDetail.Key}>{_value}");
                                break;
                            case Condition.LessThan:
                                searchs.Add($@"CustomProperties.{searchParamDetail.Key}<{_value}");
                                break;
                            case Condition.GreaterThanOrEqual:
                                searchs.Add($@"CustomProperties.{searchParamDetail.Key}>={_value}");
                                break;
                            case Condition.LessThanThanOrEqual:
                                searchs.Add($@"CustomProperties.{searchParamDetail.Key}<={_value}");
                                break;
                            case Condition.Contain:
                                searchs.Add($@"CustomProperties.{searchParamDetail.Key}.Contains({_value})");
                                break;
                        }
                    }
                }

                var internalSearch = "";
                if (searchs.Count > 1)
                {
                    switch (searchParam.InternalJoin)
                    {
                        case Message.JoinType.None:
                            break;
                        case Message.JoinType.And:
                            internalSearch = string.Join(" and ", searchs);
                            break;
                        case Message.JoinType.Or:
                            internalSearch = string.Join(" or ", searchs);
                            break;
                    }
                }
                else
                    internalSearch = searchs[0];

                if (search.Length > 0 && searchParam.ExternalJoin == Message.JoinType.None)
                    searchParam.ExternalJoin = Message.JoinType.And;

                switch (searchParam.ExternalJoin)
                {
                    case Message.JoinType.None:
                        search.Append($"({internalSearch})");
                        break;
                    case Message.JoinType.And:
                        search.Append($" and ({internalSearch})");
                        break;
                    case Message.JoinType.Or:
                        search.Append($" or ({internalSearch})");
                        break;
                }
            }

            var searchParams_where = search.ToString();
            var groupIds_where = new List<string>();
            foreach (var groupId in filterGroupIds)
            {
                groupIds_where.Add($@"GroupId==""{groupId}""");
            }
            var where = $@"UserId==""{userId}"" and GroupType=={groupType}" +
                    (groupIds_where.Count > 0 ? " and " + string.Join(" or ", groupIds_where) : "") +
                    (string.IsNullOrWhiteSpace(searchParams_where) ? "" : " and " + searchParams_where);
            return where;
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
