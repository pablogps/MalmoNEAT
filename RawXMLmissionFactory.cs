using System;
namespace RunMission
{
    public static class RawXMLmissionFactory
    {
        public static string xmlMission =
            @"
            <? xml version=""1.0"" encoding=""UTF-8"" standalone=""no"" ?>
            <Mission xmlns = ""http://ProjectMalmo.microsoft.com"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">

              <About>
                <Summary>Nothing.</Summary>
              </About>

              <ServerSection>
                <ServerInitialConditions>
                  <Time><StartTime>1</StartTime></Time>
                </ServerInitialConditions>
                <ServerHandlers>
                  <FlatWorldGenerator generatorString = ""3;7,220*1,5*3,2;3;,biome_1""/>
                  <DrawingDecorator>
                    <!-- coordinates for cuboid are inclusive -->
                    <DrawCuboid x1 = "" - 2"" y1=""46"" z1=""-2"" x2=""7"" y2=""50"" z2=""13"" type=""air"" />            <!-- limits of our arena -->
                    <DrawCuboid x1 = "" - 2"" y1=""45"" z1=""-2"" x2=""7"" y2=""45"" z2=""13"" type=""lava"" />           <!-- lava floor -->
                    <DrawCuboid x1 = ""1""  y1=""45"" z1=""1""  x2=""3"" y2=""45"" z2=""17"" type=""sandstone"" />      <!-- floor of the arena -->
                    <DrawBlock x = ""4""  y=""45"" z=""1"" type=""cobblestone"" />    <!-- the starting marker -->
                    <DrawBlock x = ""4""  y=""45"" z=""7"" type=""lapis_block"" />     <!-- the destination marker -->
                  </DrawingDecorator>
                  <ServerQuitFromTimeUp timeLimitMs = ""3000""/>
                  <ServerQuitWhenAnyAgentFinishes/>
                </ServerHandlers>
              </ServerSection>

              <AgentSection mode = ""Survival"">
                <Name>Paco</Name>
                <AgentStart>
                  <Placement x = ""4.5"" y=""46.0"" z=""1.5"" pitch=""30"" yaw=""0""/>
                </AgentStart>
                <AgentHandlers>
                  <ChatCommands/>
                  <ObservationFromFullStats/>
                  <ObservationFromGrid>
                    <Grid name = ""floor3x3"">
                      <min x = "" - 1"" y=""-1"" z=""-1""/>
                      <max x = ""1"" y=""-1"" z=""1""/>
                    </Grid>
                  </ObservationFromGrid>
                  <ContinuousMovementCommands turnSpeedDegs = ""180""/>
                </AgentHandlers>
              </AgentSection>

            </Mission>
            ";
    }
}
