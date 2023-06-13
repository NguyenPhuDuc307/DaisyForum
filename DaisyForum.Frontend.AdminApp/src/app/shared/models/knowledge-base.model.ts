export class KnowledgeBase {
    id: number;
    categoryId: number;
    categoryAlias: string;
    categoryName: string;
    title: string;
    seoAlias: string;
    description: string;
    viewCount: number;
    createDate: string;
    numberOfVotes: number;
    numberOfComments: number;
    labels: string[];
    isProcessed: boolean;
}