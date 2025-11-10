// "Module" hỏi: "Game có tài nguyên X không?"

public interface IResourceProvider
{
    bool HasResource(string resourceType, int amount);
    void SpendResource(string resourceType, int amount);
}

// "Module" hỏi: "Ai đang dùng skill? Mục tiêu là ai?"