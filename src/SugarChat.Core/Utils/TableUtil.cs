﻿using SugarChat.Core.Domain;
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
    }
}
