using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace BlogApp.Persistence.DatabaseInitializer.Seeders;

/// <summary>
/// Örnek post verilerini seed eder
/// </summary>
public class PostSeeder : BaseSeeder
{
    public PostSeeder(BlogAppDbContext context, ILogger<PostSeeder> logger) 
        : base(context, logger)
    {
    }

    public override int Order => 7; // CategorySeeder'dan sonra
    public override string Name => "Post Seeder";

    protected override async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        var seedDate = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);
        var systemUserId = SystemUsers.SystemUserId;

        // Kategori ID'leri - CategorySeeder ile eşleşmeli
        var technologyCategoryId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var programmingCategoryId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var designCategoryId = Guid.Parse("10000000-0000-0000-0000-000000000003");
        var lifestyleCategoryId = Guid.Parse("10000000-0000-0000-0000-000000000004");
        var travelCategoryId = Guid.Parse("10000000-0000-0000-0000-000000000005");

        var postDataList = new[]
        {
            new 
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), 
                Title = "Getting Started with .NET 9",
                Summary = "Explore the latest features and improvements in .NET 9 framework.",
                Body = @"# Getting Started with .NET 9

.NET 9 brings a host of exciting new features and performance improvements that make it an essential upgrade for developers. In this comprehensive guide, we'll explore the key highlights and show you how to get started.

## What's New?

- **Performance Improvements**: Up to 30% faster execution in common scenarios
- **New Language Features**: Enhanced pattern matching and improved async/await
- **Better Cloud Native Support**: Improved container integration and deployment options
- **Enhanced AI Integration**: Native support for AI and machine learning workloads

## Installation

Getting started is easy. Simply download the latest SDK from the official website and install it on your development machine.

## Conclusion

.NET 9 represents a significant step forward for the platform, making it more powerful and developer-friendly than ever before.",
                CategoryId = technologyCategoryId,
                Thumbnail = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800&h=600&fit=crop",
                IsPublished = true
            },
            new 
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), 
                Title = "Mastering Clean Architecture in C#",
                Summary = "Learn how to build maintainable and testable applications using Clean Architecture principles.",
                Body = @"# Mastering Clean Architecture in C#

Clean Architecture is a software design philosophy that emphasizes separation of concerns and independence of frameworks, UI, and databases.

## Core Principles

1. **Independence**: Your business logic should not depend on external frameworks
2. **Testability**: Business rules can be tested without UI, database, or external services
3. **UI Independence**: The UI can change easily without changing the system
4. **Database Independence**: You can swap databases without affecting business rules

## Layer Structure

- **Domain Layer**: Contains business logic and entities
- **Application Layer**: Contains use cases and application logic
- **Infrastructure Layer**: Contains external concerns like databases and APIs
- **Presentation Layer**: Contains UI components

Clean Architecture helps teams build scalable and maintainable applications that stand the test of time.",
                CategoryId = programmingCategoryId,
                Thumbnail = "https://images.unsplash.com/photo-1542831371-29b0f74f9713?w=800&h=600&fit=crop",
                IsPublished = true
            },
            new 
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), 
                Title = "Modern UI/UX Design Trends in 2025",
                Summary = "Discover the hottest design trends shaping user experiences this year.",
                Body = @"# Modern UI/UX Design Trends in 2025

The design landscape is constantly evolving, and 2025 brings fresh perspectives on how we create digital experiences.

## Top Trends

### 1. Neumorphism 2.0
Soft UI is making a comeback with improved accessibility and contrast ratios.

### 2. AI-Powered Personalization
Interfaces that adapt to individual user preferences in real-time.

### 3. Immersive 3D Elements
Strategic use of 3D graphics to create depth and engagement.

### 4. Dark Mode First
Designing for dark mode as the primary experience, not an afterthought.

### 5. Micro-interactions
Subtle animations that provide feedback and delight users.

## Implementation Tips

Start small and test with real users. Not every trend fits every project, so choose wisely based on your audience and goals.",
                CategoryId = designCategoryId,
                Thumbnail = "https://images.unsplash.com/photo-1561070791-2526d30994b5?w=800&h=600&fit=crop",
                IsPublished = true
            },
            new 
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), 
                Title = "Work-Life Balance in Tech: A Guide",
                Summary = "Practical strategies for maintaining healthy work-life balance in the tech industry.",
                Body = @"# Work-Life Balance in Tech: A Guide

The tech industry is known for its demanding nature, but maintaining a healthy work-life balance is crucial for long-term success and happiness.

## Key Strategies

### Set Boundaries
- Define clear work hours
- Turn off notifications after hours
- Create a dedicated workspace

### Prioritize Health
- Regular exercise routine
- Adequate sleep (7-9 hours)
- Healthy eating habits

### Time Management
- Use the Pomodoro Technique
- Block time for deep work
- Learn to say no

### Disconnect Regularly
- Weekend digital detox
- Vacation without work email
- Hobby time

## The Bottom Line

Your productivity and creativity increase when you're well-rested and fulfilled outside of work. Invest in yourself beyond your career.",
                CategoryId = lifestyleCategoryId,
                Thumbnail = "https://images.unsplash.com/photo-1499750310107-5fef28a66643?w=800&h=600&fit=crop",
                IsPublished = true
            },
            new 
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), 
                Title = "Digital Nomad Destinations for Developers",
                Summary = "Top cities around the world that offer great infrastructure and lifestyle for remote developers.",
                Body = @"# Digital Nomad Destinations for Developers

Working remotely opens up a world of possibilities. Here are the best destinations for developers who want to combine work with adventure.

## Top Destinations

### 1. Lisbon, Portugal
- Excellent internet infrastructure
- Vibrant tech community
- Affordable cost of living
- Amazing food and culture

### 2. Chiang Mai, Thailand
- Ultra-low cost of living
- Strong digital nomad community
- Great coworking spaces
- Tropical climate

### 3. Medellín, Colombia
- Perfect weather year-round
- Growing tech scene
- Affordable housing
- Friendly locals

### 4. Tallinn, Estonia
- Digital nomad visa available
- Advanced digital infrastructure
- Beautiful medieval city
- Gateway to Europe

### 5. Bali, Indonesia
- Beach paradise
- Established nomad infrastructure
- Wellness-focused lifestyle
- Inspiring environment

## Tips for Success

Research visa requirements, time zones for client calls, and reliable internet before committing to a location. Join local communities to make the most of your experience.",
                CategoryId = travelCategoryId,
                Thumbnail = "https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=800&h=600&fit=crop",
                IsPublished = true
            },
            new 
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000006"), 
                Title = "Understanding Microservices Architecture",
                Summary = "A comprehensive guide to building scalable applications with microservices.",
                Body = @"# Understanding Microservices Architecture

Microservices have revolutionized how we build and deploy large-scale applications. Let's dive into what makes this architecture pattern so powerful.

## What Are Microservices?

Microservices is an architectural style that structures an application as a collection of small, autonomous services modeled around a business domain.

## Key Benefits

- **Scalability**: Scale services independently based on demand
- **Flexibility**: Use different technologies for different services
- **Resilience**: Failure in one service doesn't bring down the entire system
- **Faster Deployment**: Deploy services independently without affecting others

## Challenges

- **Complexity**: Distributed systems are inherently complex
- **Data Management**: Each service should have its own database
- **Network Issues**: Handle latency and failures gracefully
- **Testing**: Integration testing becomes more complex

## Best Practices

1. Design for failure
2. Use API gateways
3. Implement circuit breakers
4. Monitor everything
5. Use containerization

Microservices aren't for every project, but when applied correctly, they can provide significant advantages for large, complex applications.",
                CategoryId = programmingCategoryId,
                Thumbnail = "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=800&h=600&fit=crop",
                IsPublished = true
            }
        };

        var posts = new List<Post>();

        foreach (var postData in postDataList)
        {
            var post = Post.Create(
                title: postData.Title,
                body: postData.Body,
                summary: postData.Summary,
                categoryId: postData.CategoryId,
                thumbnail: postData.Thumbnail
            );

            // Post'u publish et
            if (postData.IsPublished)
            {
                post.Publish();
            }

            // Sabit ID ve tarihleri EF Core ile set et
            var entry = Context.Entry(post);
            entry.Property("Id").CurrentValue = postData.Id;
            entry.Property("CreatedDate").CurrentValue = seedDate;
            entry.Property("CreatedById").CurrentValue = systemUserId;
            entry.Property("IsDeleted").CurrentValue = false;

            posts.Add(post);
        }

        await AddRangeIfNotExistsAsync(posts, p => (Guid)Context.Entry(p).Property("Id").CurrentValue!, cancellationToken);
    }
}
