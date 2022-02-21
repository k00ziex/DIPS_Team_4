import unittest
from ImprovedBullyProcess import ImprovedBullyProcess

class MyTestCase(unittest.TestCase):
    def test_something(self):

        ListOfP = []

        p1 = ImprovedBullyProcess(10)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(9)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(11)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(2)
        ListOfP.append(p4)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)

        p1.startElection()

        print(p1.whoseLeader)
        print(p2.whoseLeader)
        print(p3.whoseLeader)
        print(p4.whoseLeader)

    def test_starterNodeIsHighest_becomesLeader(self):
        ListOfP = []

        p1 = ImprovedBullyProcess(10)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(9)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(11)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(2)
        ListOfP.append(p4)
        p5 = ImprovedBullyProcess(4)
        ListOfP.append(p5)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)
        p5.setNeighbors(ListOfP)

        p3.startElection()

        self.assertTrue(p1.whoseLeader == p3.id)
        self.assertTrue(p2.whoseLeader == p3.id)
        self.assertTrue(p3.whoseLeader == p3.id)
        self.assertTrue(p4.whoseLeader == p3.id)
        self.assertTrue(p5.whoseLeader == p3.id)

    def test_starterNodeIsLowest_highestBecomeLeader(self):
        ListOfP = []

        p1 = ImprovedBullyProcess(3)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(12)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(4)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(1)
        ListOfP.append(p4)
        p5 = ImprovedBullyProcess(5)
        ListOfP.append(p5)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)
        p5.setNeighbors(ListOfP)

        p4.startElection()

        self.assertTrue(p1.whoseLeader == p2.id)
        self.assertTrue(p2.whoseLeader == p2.id)
        self.assertTrue(p3.whoseLeader == p2.id)
        self.assertTrue(p4.whoseLeader == p2.id)
        self.assertTrue(p5.whoseLeader == p2.id)

    def test_highestNodeIsDead_secondHighestBecomeLeader(self):
        ListOfP = []

        p1 = ImprovedBullyProcess(3)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(12)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(4)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(1)
        ListOfP.append(p4)
        p5 = ImprovedBullyProcess(5)
        ListOfP.append(p5)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)
        p5.setNeighbors(ListOfP)

        p2.killNode()

        p4.startElection()

        self.assertTrue(p1.whoseLeader == p5.id)
        self.assertTrue(p3.whoseLeader == p5.id)
        self.assertTrue(p4.whoseLeader == p5.id)
        self.assertTrue(p5.whoseLeader == p5.id)

    def aNodeIsDead_DoesNotLearnLeader(self):
        pass
    def allNodesExceptOneIsDead_OneBecomesLeader(self):
        pass
    def onlyNodesLowerAreAlive_StarterBecomesLeader(self):
        pass





if __name__ == '__main__':
    unittest.main()
