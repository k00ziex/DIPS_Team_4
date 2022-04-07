import unittest
from ImprovedBullyProcess import ImprovedBullyProcess

class TestCases(unittest.TestCase):
    def test_manualtesting(self):

        ListOfP = []
        networkMessagesSent = [0]

        p1 = ImprovedBullyProcess(10, networkMessagesSent)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(9, networkMessagesSent)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(11, networkMessagesSent)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(2, networkMessagesSent)
        ListOfP.append(p4)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)

        p1.startElection()

        print(f"{self.id()}: Leader P1: {p1.whoseLeader}")
        print(f"{self.id()}: Leader P2: {p2.whoseLeader}")
        print(f"{self.id()}: Leader P3: {p3.whoseLeader}")
        print(f"{self.id()}: Leader P4: {p4.whoseLeader}")
        print(f"{self.id()}: Messages Sent in Network: {networkMessagesSent[0]}")

    def test_starterNodeIsHighest_becomesLeader(self):
        ListOfP = []
        networkMessagesSent = [0]

        p1 = ImprovedBullyProcess(10, networkMessagesSent)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(9, networkMessagesSent)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(11, networkMessagesSent)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(2, networkMessagesSent)
        ListOfP.append(p4)
        p5 = ImprovedBullyProcess(4, networkMessagesSent)
        ListOfP.append(p5)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)
        p5.setNeighbors(ListOfP)

        p3.startElection()

        self.assertEqual(p1.whoseLeader, p3.id)
        self.assertEqual(p2.whoseLeader, p3.id)
        self.assertEqual(p3.whoseLeader, p3.id)
        self.assertEqual(p4.whoseLeader, p3.id)
        self.assertEqual(p5.whoseLeader, p3.id)
        print(f"{self.id()}: Messages Sent in Network: {networkMessagesSent[0]}")

    def test_starterNodeIsLowest_highestBecomeLeader(self):

        ListOfP = []
        networkMessagesSent = [0]

        p1 = ImprovedBullyProcess(3, networkMessagesSent)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(12, networkMessagesSent)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(4, networkMessagesSent)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(1, networkMessagesSent)
        ListOfP.append(p4)
        p5 = ImprovedBullyProcess(5, networkMessagesSent)
        ListOfP.append(p5)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)
        p5.setNeighbors(ListOfP)

        p4.startElection()

        self.assertEqual(p1.whoseLeader, p2.id)
        self.assertEqual(p2.whoseLeader, p2.id)
        self.assertEqual(p3.whoseLeader, p2.id)
        self.assertEqual(p4.whoseLeader, p2.id)
        self.assertEqual(p5.whoseLeader, p2.id)
        print(f"{self.id()}: Messages Sent in Network: {networkMessagesSent[0]}")

    def test_highestNodeIsDead_secondHighestBecomeLeader(self):

        networkMessagesSent = [0]
        ListOfP = []

        p1 = ImprovedBullyProcess(3, networkMessagesSent)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(12, networkMessagesSent)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(4, networkMessagesSent)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(1, networkMessagesSent)
        ListOfP.append(p4)
        p5 = ImprovedBullyProcess(5, networkMessagesSent)
        ListOfP.append(p5)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)
        p5.setNeighbors(ListOfP)

        p2.killNode()

        p4.startElection()

        self.assertEqual(p1.whoseLeader, p5.id)
        self.assertEqual(p3.whoseLeader, p5.id)
        self.assertEqual(p4.whoseLeader, p5.id)
        self.assertEqual(p5.whoseLeader, p5.id)
        print(f"{self.id()}: Messages Sent in Network: {networkMessagesSent[0]}")

    def test_aNodeIsDead_DoesNotLearnLeader(self):
        networkMessagesSent = [0]
        ListOfP = []

        p1 = ImprovedBullyProcess(4536456, networkMessagesSent)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(124, networkMessagesSent)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(3457568567, networkMessagesSent)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(14, networkMessagesSent)
        ListOfP.append(p4)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)
        p2.killNode()

        p4.startElection()

        self.assertEqual(p2.whoseLeader, int)
        print(f"{self.id()}: Messages Sent in Network: {networkMessagesSent[0]}")


    def test_allNodesExceptOneIsDead_OneBecomesLeader(self):
        networkMessagesSent = [0]
        ListOfP = []

        p1 = ImprovedBullyProcess(4536456, networkMessagesSent)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(124, networkMessagesSent)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(3457568567, networkMessagesSent)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(14, networkMessagesSent)
        ListOfP.append(p4)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)

        p1.killNode()
        p3.killNode()
        p4.killNode()
        p2.startElection()

        self.assertEqual(p2.whoseLeader, 124)
        print(f"{self.id()}: Messages Sent in Network: {networkMessagesSent[0]}")
    
    def test_onlyNodesLowerAreAlive_StarterBecomesLeader(self):
        networkMessagesSent = [0]
        ListOfP = []

        p1 = ImprovedBullyProcess(10, networkMessagesSent)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(9, networkMessagesSent)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(4, networkMessagesSent)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(1, networkMessagesSent)
        ListOfP.append(p4)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)

        p1.killNode()

        p2.startElection()

        self.assertEqual(p2.whoseLeader, 9)
        self.assertEqual(p3.whoseLeader, 9)
        self.assertEqual(p4.whoseLeader, 9)

        self.assertEqual(p1.whoseLeader, int) # Not set

        print(f"{self.id()}: Messages Sent in Network: {networkMessagesSent[0]}")




if __name__ == '__main__':
    unittest.main()
