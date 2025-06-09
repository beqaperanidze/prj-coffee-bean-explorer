# Refactoring Documentation

## 1. Refactor an Existing Service (CQRS, Dependency Injection, MediatR)

**Before:**

```csharp
public class ReviewService : IReviewService
{
    public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync() { ... }
    public async Task<(ReviewDto?, string?)> CreateReviewAsync(CreateReviewDto dto, int userId) { ... }
}
```

**After:**

```csharp
public record GetAllReviewsQuery : IRequest<IEnumerable<ReviewDto>>;
public record CreateReviewCommand(CreateReviewDto Dto, int UserId) : IRequest<(ReviewDto?, string?)>;
```

## 2. Add AutoMapper

**Before:**

```csharp
// Manual mapping
var reviewDto = new ReviewDto
{
    Id = review.Id,
    Rating = review.Rating,
    Comment = review.Comment,
    // ...
};
```

**After:**

```csharp
// AutoMapper profile
public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewDto>();
    }
}

// Usage
var reviewDto = _mapper.Map<ReviewDto>(review);
```

## 3. Add FluentValidation

**Before:**

```csharp
// Manual validation in controller or service
if (dto.Rating < 1 || dto.Rating > 5) return BadRequest("Invalid rating");
```

**After:**

```csharp
public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).NotEmpty();
    }
}

services.AddValidatorsFromAssemblyContaining<CreateReviewDtoValidator>();
```

## 4. Apply Modern C# Features

**Before:**

```csharp
public class ReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
}
```

**After:**

```csharp
public record ReviewDto
{
    public int Id { get; init; }
    public int Rating { get; init; }
    public string Comment { get; init; }
}
```

**Before:**

```csharp
if (beanId != null && userId != null) { ... }
else if (beanId != null) { ... }
else if (userId != null) { ... }
else { ... }
```

**After:**

```csharp
var reviews = (beanId, userId) switch
{
    ({ } bid, { } uid) => ...,
    ({ } bid, null) => ...,
    (null, { } uid) => ...,
    (null, null) => ...
};
```
