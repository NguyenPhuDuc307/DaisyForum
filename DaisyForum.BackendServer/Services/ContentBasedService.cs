using System.Globalization;
using DaisyForum.BackendServer.Constants;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Helpers;
using DaisyForum.ViewModels.Contents;
using Microsoft.VisualBasic.FileIO;

namespace DaisyForum.BackendServer.Services;

public class ContentBasedService : IContentBasedService
{
    private readonly ApplicationDbContext _context;
    private readonly ISequenceService _sequenceService;
    private readonly IStorageService _storageService;
    private readonly IViewRenderService _viewRenderService;
    private readonly ICacheService _cacheService;

    public ContentBasedService(ApplicationDbContext context,
            ISequenceService sequenceService,
            IStorageService storageService,
            IViewRenderService viewRenderService,
            ICacheService cacheService)
    {
        _context = context;
        _sequenceService = sequenceService;
        _storageService = storageService;
        _viewRenderService = viewRenderService;
        _cacheService = cacheService;
    }
    public List<KnowledgeBasesFromCSV> GetData(string filePath)
    {
        using (TextFieldParser parser = new TextFieldParser(filePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            // skip the header line
            parser.ReadLine();
            int idCounter = 1;

            List<KnowledgeBasesFromCSV> dataList = new List<KnowledgeBasesFromCSV>();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                KnowledgeBasesFromCSV data = new KnowledgeBasesFromCSV();
                data.Id = int.Parse(fields[0]);
                data.Title = fields[1];
                data.Body = fields[2];
                data.Tags = fields[3].Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                data.CreationDate = DateTime.ParseExact(fields[4], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                data.Y = fields[5];
                dataList.Add(data);
                idCounter++;
            }

            return dataList;
        }
    }

    private static KnowledgeBase CreateKnowledgeBaseEntity(KnowledgeBaseCreateRequest request)
    {
        var entity = new KnowledgeBase()
        {
            CategoryId = request.CategoryId,

            Title = request.Title,

            SeoAlias = request.SeoAlias,

            Description = request.Description,

            Environment = request.Environment,

            Problem = request.Problem,

            StepToReproduce = request.StepToReproduce,

            ErrorMessage = request.ErrorMessage,

            Workaround = request.Workaround,

            Note = request.Note
        };
        if (request.Labels?.Length > 0)
        {
            entity.Labels = string.Join(',', request.Labels);
        }
        return entity;
    }

    private async Task ProcessLabel(KnowledgeBaseCreateRequest request, KnowledgeBase knowledgeBase)
    {
        if (request.Labels != null)
        {
            foreach (var labelText in request.Labels)
            {
                if (labelText == null) continue;
                var labelId = TextHelper.ToUnsignedString(labelText.ToString()); var existingLabel = await _context.Labels.FindAsync(labelId);
                if (existingLabel == null)
                {
                    var labelEntity = new Label()
                    {
                        Id = labelId,
                        Name = labelText.ToString()
                    };
                    _context.Labels.Add(labelEntity);
                }
                if (await _context.LabelInKnowledgeBases.FindAsync(labelId, knowledgeBase.Id) == null)
                {
                    _context.LabelInKnowledgeBases.Add(new LabelInKnowledgeBase()
                    {
                        KnowledgeBaseId = knowledgeBase.Id,
                        LabelId = labelId
                    });
                }
            }
        }
    }

    public async Task SeedData(List<KnowledgeBaseCreateRequest> requests, string UserId)
    {
        foreach (var request in requests)
        {
            KnowledgeBase knowledgeBase = CreateKnowledgeBaseEntity(request);
            knowledgeBase.OwnerUserId = UserId;
            if (string.IsNullOrEmpty(knowledgeBase.SeoAlias))
            {
                knowledgeBase.SeoAlias = TextHelper.ToUnsignedString(knowledgeBase.Title != null ? knowledgeBase.Title : "");
            }
            knowledgeBase.Id = await _sequenceService.GetKnowledgeBaseNewId();

            _context.KnowledgeBases.Add(knowledgeBase);

            //Process label
            if (request.Labels?.Length > 0)
            {
                await ProcessLabel(request, knowledgeBase);
            }

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync(CacheConstants.LatestKnowledgeBases);
                await _cacheService.RemoveAsync(CacheConstants.PopularKnowledgeBases);
                await _cacheService.RemoveAsync(CacheConstants.PopularLabels);
            }
        }
    }
}