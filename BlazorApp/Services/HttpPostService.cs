using System.Text.Json;
using ApiContracts.DTOs;
using Entities;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<Post> CreateAsync(CreatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("posts", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        
        return JsonSerializer.Deserialize<Post>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<Post> GetSingleAsync(int id, bool includeComments = false, bool includeAuthor = false)
    {
        string query = $"posts/{id}?includeComments={includeComments}&includeAuthor={includeAuthor}";
        
        HttpResponseMessage httpResponse = await client.GetAsync(query);
        string response = await httpResponse.Content.ReadAsStringAsync();
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        
        return JsonSerializer.Deserialize<Post>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<IEnumerable<Post>> GetManyAsync(int? userId = null, string? search = null, int skip = 0, int take = 50, bool includeComments = false, bool includeAuthor = false)
    {
        string query = $"posts?skip={skip}&take={take}&includeComments={includeComments}&includeAuthor={includeAuthor}";
        
        if (userId.HasValue)
        {
            query += $"&userId={userId.Value}";
        }
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query += $"&search={search}";
        }
        
        HttpResponseMessage httpResponse = await client.GetAsync(query);
        string response = await httpResponse.Content.ReadAsStringAsync();
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        
        return JsonSerializer.Deserialize<IEnumerable<Post>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task UpdateAsync(int id, UpdatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"posts/{id}", request);
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }

    public async Task DeleteAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"posts/{id}");
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }
}