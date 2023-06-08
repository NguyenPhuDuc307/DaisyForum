namespace DaisyForum.ViewModels.Contents;

public class TreeNode
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentId { get; set; }
    public string? SeoAlias { get; set; }
    public List<TreeNode> Children { get; set; } = new List<TreeNode>();

    public TreeNode(int id, string? name, int? parentId, string? seoAlias, List<TreeNode> children)
    {
        Name = name;
        Id = id;
        ParentId = parentId;
        Children = children;
        SeoAlias = seoAlias;
    }

    public static List<TreeNode> ConvertToTreeNodes(List<CategoryViewModel> categories, int? parentId = null)
    {
        var treeNodes = new List<TreeNode>();
        foreach (var category in categories.Where(c => c.ParentId == parentId))
        {
            var children = ConvertToTreeNodes(categories, category.Id);
            var treeNode = new TreeNode(category.Id, category.Name, category.ParentId, category.SeoAlias, children);
            treeNodes.Add(treeNode);
        }
        return treeNodes;
    }
}

