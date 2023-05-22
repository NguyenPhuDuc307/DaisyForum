using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DaisyForum.WebPortal.Services
{
    public class KnowledgeBaseApiClient : BaseApiClient, IKnowledgeBaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KnowledgeBaseApiClient(IHttpClientFactory httpClientFactory,
          IConfiguration configuration,
          IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Pagination<CommentViewModel>> GetCommentsTree(int knowledgeBaseId, int pageIndex, int pageSize)
        {
            return await GetAsync<Pagination<CommentViewModel>>($"/api/knowledgeBases/{knowledgeBaseId}/comments/tree?pageIndex={pageIndex}&pageSize={pageSize}");
        }

        public async Task<Pagination<CommentViewModel>> GetRepliedComments(int knowledgeBaseId, int rootCommentId, int pageIndex, int pageSize)
        {
            return await GetAsync<Pagination<CommentViewModel>>($"/api/knowledgeBases/{knowledgeBaseId}/comments/{rootCommentId}/replied?pageIndex={pageIndex}&pageSize={pageSize}");
        }

        public async Task<KnowledgeBaseViewModel> GetKnowledgeBaseDetail(int id)
        {
            return await GetAsync<KnowledgeBaseViewModel>($"/api/knowledgeBases/{id}");
        }

        public async Task<Pagination<KnowledgeBaseQuickViewModel>> GetKnowledgeBasesByCategoryId(int categoryId, int pageIndex, int pageSize)
        {
            var apiUrl = $"/api/knowledgeBases/filter?categoryId={categoryId}&pageIndex={pageIndex}&pageSize={pageSize}";
            return await GetAsync<Pagination<KnowledgeBaseQuickViewModel>>(apiUrl);
        }

        public async Task<Pagination<KnowledgeBaseQuickViewModel>> GetKnowledgeBasesByTagId(string tagId, int pageIndex, int pageSize)
        {
            var apiUrl = $"/api/knowledgeBases/tags/{tagId}?pageIndex={pageIndex}&pageSize={pageSize}";
            return await GetAsync<Pagination<KnowledgeBaseQuickViewModel>>(apiUrl);
        }

        public async Task<List<LabelViewModel>> GetLabelsByKnowledgeBaseId(int id)
        {
            return await GetListAsync<LabelViewModel>($"/api/knowledgeBases/{id}/labels");
        }

        public async Task<List<KnowledgeBaseQuickViewModel>> GetLatestKnowledgeBases(int take)
        {
            return await GetListAsync<KnowledgeBaseQuickViewModel>($"/api/knowledgeBases/latest/{take}");
        }

        public async Task<List<KnowledgeBaseQuickViewModel>> GetPopularKnowledgeBases(int take)
        {
            return await GetListAsync<KnowledgeBaseQuickViewModel>($"/api/knowledgeBases/popular/{take}");
        }

        public async Task<List<CommentViewModel>> GetRecentComments(int take)
        {
            return await GetListAsync<CommentViewModel>($"/api/knowledgeBases/comments/recent/{take}");
        }

        public async Task<CommentViewModel> PostComment(CommentCreateRequest request)
        {
            return await PostAsync<CommentCreateRequest, CommentViewModel>($"/api/knowledgeBases/{request.KnowledgeBaseId}/comments", request);
        }

        public async Task<bool> DeleteComment(int knowledgeBaseId, int commentId)
        {
            return await DeleteAsync<bool>($"/api/knowledgeBases/{knowledgeBaseId}/comments/{commentId}");
        }

        public async Task<bool> PostKnowledgeBase(KnowledgeBaseCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            using var requestContent = new MultipartFormDataContent();

            if (request.Attachments?.Count > 0)
            {
                foreach (var item in request.Attachments)
                {
                    byte[] data;
                    using (var br = new BinaryReader(item.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)item.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "attachments", item.FileName);
                }
            }
            requestContent.Add(new StringContent(request.CategoryId.ToString()), "categoryId");
            requestContent.Add(new StringContent(request.Title != null ? request.Title.ToString() : ""), "title");
            requestContent.Add(new StringContent(request.Problem != null ? request.Problem.ToString() : ""), "problem");
            requestContent.Add(new StringContent(request.Note != null ? request.Note.ToString() : ""), "note");
            requestContent.Add(new StringContent(request.Description != null ? request.Description.ToString() : ""), "description");
            requestContent.Add(new StringContent(request.Environment != null ? request.Environment.ToString() : ""), "environment");
            requestContent.Add(new StringContent(request.StepToReproduce != null ? request.StepToReproduce.ToString() : ""), "stepToReproduce");
            requestContent.Add(new StringContent(request.ErrorMessage != null ? request.ErrorMessage.ToString() : ""), "errorMessage");
            requestContent.Add(new StringContent(request.Workaround != null ? request.Workaround.ToString() : ""), "workaround");
            if (request.Labels?.Length > 0)
            {
                foreach (var label in request.Labels)
                {
                    requestContent.Add(new StringContent(label), "labels");
                }
            }

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"/api/knowledgeBases/", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutKnowledgeBase(int id, KnowledgeBaseCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
            using var requestContent = new MultipartFormDataContent();

            if (request.Attachments?.Count > 0)
            {
                foreach (var item in request.Attachments)
                {
                    byte[] data;
                    using (var br = new BinaryReader(item.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)item.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "attachments", item.FileName);
                }
            }
            requestContent.Add(new StringContent(request.CategoryId.ToString()), "categoryId");
            requestContent.Add(new StringContent(request.Title.ToString()), "title");
            requestContent.Add(new StringContent(request.Problem.ToString()), "problem");
            requestContent.Add(new StringContent(request.Note.ToString()), "note");
            requestContent.Add(new StringContent(request.Description.ToString()), "description");
            requestContent.Add(new StringContent(request.Environment.ToString()), "environment");
            requestContent.Add(new StringContent(request.StepToReproduce.ToString()), "stepToReproduce");
            requestContent.Add(new StringContent(request.ErrorMessage.ToString()), "errorMessage");
            requestContent.Add(new StringContent(request.Workaround.ToString()), "workaround");
            if (request.Labels?.Length > 0)
            {
                foreach (var label in request.Labels)
                {
                    requestContent.Add(new StringContent(label), "labels");
                }
            }

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"/api/knowledgeBases/{id}", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<Pagination<KnowledgeBaseQuickViewModel>> SearchKnowledgeBase(string keyword, int pageIndex, int pageSize)
        {
            var apiUrl = $"/api/knowledgeBases/filter?filter={keyword}&pageIndex={pageIndex}&pageSize={pageSize}";
            return await GetAsync<Pagination<KnowledgeBaseQuickViewModel>>(apiUrl);
        }

        public async Task<bool> UpdateViewCount(int id)
        {
            return await PutAsync<object, bool>($"/api/knowledgeBases/{id}/view-count", null, false);
        }

        public async Task<int> PostVote(VoteCreateRequest request)
        {
            return await PostAsync<VoteCreateRequest, int>($"/api/knowledgeBases/{request.KnowledgeBaseId}/votes", null);
        }

        public async Task<ReportViewModel> PostReport(ReportCreateRequest request)
        {
            return await PostAsync<ReportCreateRequest, ReportViewModel>($"/api/knowledgeBases/{request.KnowledgeBaseId}/reports", request);
        }
    }
}