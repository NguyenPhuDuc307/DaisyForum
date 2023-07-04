namespace DaisyForum.ViewModels.Statistics;
public class MonthlyCommentsViewModel
{
    public int Month { get; set; }
    public int NumberOfComments { get; set; }
    public int NumberOfNegativeComments { get; set; }
    public int NumberOfPositiveComments { get; set; }
    public int NumberOfNeutralComments { get; set; }
}