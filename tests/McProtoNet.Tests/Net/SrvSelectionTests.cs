using System.Diagnostics;

namespace McProtoNet.Tests.Net;

/// <summary>
///     RFC 2782 selection with the draw pinned down: <c>roll</c> hands the algorithm the number it
///     would otherwise take from <see cref="Random" />, so every outcome here is exact.
/// </summary>
public class SrvSelectionTests
{
    private static readonly Func<int, int> Lowest = _ => 0;
    private static readonly Func<int, int> Highest = max => max - 1;

    [Fact]
    public void Select_NoRecords_ReturnsNull()
    {
        Assert.Null(SrvResolver.Select([], Lowest));
    }

    [Fact]
    public void Select_LowestPriorityWins_WhateverTheWeights()
    {
        SrvResult[] records =
        [
            new("heavy.example.com", 25565, 20, 1000),
            new("light.example.com", 25566, 5, 1),
            new("middle.example.com", 25567, 10, 500)
        ];

        Assert.Equal("light.example.com", SrvResolver.Select(records, Lowest)!.Value.Target);
        Assert.Equal("light.example.com", SrvResolver.Select(records, Highest)!.Value.Target);
    }

    [Fact]
    public void Select_InsideOnePriority_FollowsTheWeightedDraw()
    {
        SrvResult[] records =
        [
            new("a.example.com", 1, 0, 10),
            new("b.example.com", 2, 0, 20),
            new("c.example.com", 3, 0, 70)
        ];

        // Running weights are 10, 30, 100; a draw of 0 stops at the first, a draw of 100 at the last.
        Assert.Equal("a.example.com", SrvResolver.Select(records, Lowest)!.Value.Target);
        Assert.Equal("c.example.com", SrvResolver.Select(records, Highest)!.Value.Target);
        Assert.Equal("a.example.com", SrvResolver.Select(records, _ => 10)!.Value.Target);
        Assert.Equal("b.example.com", SrvResolver.Select(records, _ => 11)!.Value.Target);
        Assert.Equal("b.example.com", SrvResolver.Select(records, _ => 30)!.Value.Target);
        Assert.Equal("c.example.com", SrvResolver.Select(records, _ => 31)!.Value.Target);
    }

    [Fact]
    public void Select_HigherPriorityRecordsNeverEnterTheDraw()
    {
        SrvResult[] records =
        [
            new("primary.example.com", 25565, 0, 1),
            new("backup.example.com", 25566, 1, 1000)
        ];

        for (var draw = 0; draw < 8; draw++)
        {
            var point = draw;
            Assert.Equal("primary.example.com", SrvResolver.Select(records, _ => point)!.Value.Target);
        }
    }

    [Fact]
    public void Select_AllWeightsZero_DrawsUniformly()
    {
        SrvResult[] records =
        [
            new("a.example.com", 1, 0, 0),
            new("b.example.com", 2, 0, 0),
            new("c.example.com", 3, 0, 0)
        ];

        Assert.Equal("a.example.com", SrvResolver.Select(records, Lowest)!.Value.Target);
        Assert.Equal("c.example.com", SrvResolver.Select(records, Highest)!.Value.Target);
        Assert.Equal("b.example.com", SrvResolver.Select(records, _ => 1)!.Value.Target);
    }

    /// <summary>
    ///     RFC 2782 gives a weight-0 record a small chance rather than none, by ordering it first in the
    ///     group. Only the lowest draw picks it; everything else goes to the record carrying the weight.
    /// </summary>
    [Fact]
    public void Select_ZeroWeightBesideWeighted_KeepsItsSmallChance()
    {
        SrvResult[] records =
        [
            new("weighted.example.com", 1, 0, 100),
            new("spare.example.com", 2, 0, 0)
        ];

        Assert.Equal("spare.example.com", SrvResolver.Select(records, Lowest)!.Value.Target);
        Assert.Equal("weighted.example.com", SrvResolver.Select(records, _ => 1)!.Value.Target);
        Assert.Equal("weighted.example.com", SrvResolver.Select(records, Highest)!.Value.Target);

        // One draw in 101 lands on the spare: a share, not a lockout.
        var draws = Enumerable.Range(0, 101)
            .Select(point => SrvResolver.Select(records, _ => point)!.Value.Target)
            .ToList();
        Assert.Equal(1, draws.Count(target => target == "spare.example.com"));
        Assert.Equal(100, draws.Count(target => target == "weighted.example.com"));
    }

    [Fact]
    public void Select_SingleRecord_SkipsTheDrawEntirely()
    {
        SrvResult[] records = [new("only.example.com", 25565, 7, 0)];

        Assert.Equal(records[0], SrvResolver.Select(records, _ => throw new UnreachableException()));
    }
}
