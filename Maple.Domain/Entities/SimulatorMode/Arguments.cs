namespace Maple.Domain.Entities.SimulatorMode;

public class Arguments
{
    public AcSweepArguments AcSweep { get; set; }
    public DcSweepArguments DcSweep { get; set; }
    public TransientArguments Transient { get; set; }
}

public class AcSweepArguments
{
    public double Initial { get; set; }
    public double Final { get; set; }
    public int PointsPerDecade { get; set; }
}

public class DcSweepArguments
{
    public double Initial { get; set; }
    public double Final { get; set; }
    public double Delta { get; set; }
}

public class TransientArguments
{
    public double Step { get; set; }
    public double Final { get; set; }
}