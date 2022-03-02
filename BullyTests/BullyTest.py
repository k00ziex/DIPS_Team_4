import unittest
from OriginalBullyProcess import OriginalBullyProcess


class Basic_Election(unittest.TestCase):
    def test_HighestElectedLeader_When_LowestStartsElection(self):
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)
        node3 = OriginalBullyProcess(3)
        node4 = OriginalBullyProcess(4)
        node5 = OriginalBullyProcess(5)

        nodeArray = [node1, node2, node3, node4, node5]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        node1.startElection()

        self.assertEqual(True, node5.isLeader())
        self.assertEqual(False, node4.isLeader())
        self.assertEqual(False, node3.isLeader())
        self.assertEqual(False, node2.isLeader())
        self.assertEqual(False, node1.isLeader())


    def test_HighestElectedLeader_When_HighestStartElection(self):
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)
        node3 = OriginalBullyProcess(3)
        node4 = OriginalBullyProcess(4)
        node5 = OriginalBullyProcess(5)

        nodeArray = [node1, node2, node3, node4, node5]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        node5.startElection()

        self.assertEqual(True, node5.isLeader())
        self.assertEqual(False, node4.isLeader())
        self.assertEqual(False, node3.isLeader())
        self.assertEqual(False, node2.isLeader())
        self.assertEqual(False, node1.isLeader())


    def test_HighestElectedLeader_When_MiddleStartsElection(self):
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)
        node3 = OriginalBullyProcess(3)
        node4 = OriginalBullyProcess(4)
        node5 = OriginalBullyProcess(5)

        nodeArray = [node1, node2, node3, node4, node5]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        node3.startElection()

        self.assertEqual(True, node5.isLeader())
        self.assertEqual(False, node4.isLeader())
        self.assertEqual(False, node3.isLeader())
        self.assertEqual(False, node2.isLeader())
        self.assertEqual(False, node1.isLeader())


    def test_SecondHighestElected_When_HighestIsDead(self):
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)
        node3 = OriginalBullyProcess(3)

        nodeArray = [node1, node2, node3]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        node3.killNode()

        # Assert that second highest node is elected when lowest starts election
        node1.startElection()
        self.assertEqual(False, node3.isLeader())
        self.assertEqual(True, node2.isLeader())
        self.assertEqual(False, node1.isLeader())

        # Assert that second highest node is elected when second highest starts election
        node2.startElection()
        self.assertEqual(False, node3.isLeader())
        self.assertEqual(True, node2.isLeader())
        self.assertEqual(False, node1.isLeader())
        

    def test_Node2Then3Elected_When_Node3KilledAndStarted(self):
        
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)
        node3 = OriginalBullyProcess(3)

        nodeArray = [node1, node2, node3]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        node3.killNode()
        node1.startElection()

        # Assert node 2 being elected while 3 is dead
        self.assertEqual(False, node3.isLeader())
        self.assertEqual(True, node2.isLeader())
        self.assertEqual(False, node1.isLeader())

        node3.startNode()
        
        # Assert that node 3 takes leadership when it starts back up
        self.assertEqual(True, node3.isLeader())
        self.assertEqual(False, node2.isLeader())
        self.assertEqual(False, node1.isLeader())

    def test_NodeDoesNotLearnLeader_When_NodeIsDead(self):
        
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)
        node3 = OriginalBullyProcess(3)

        nodeArray = [node1, node2, node3]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        node2.killNode()
        node1.startElection()
        
        # Assert that highest is leader and that the killed node does not know leader
        self.assertEqual(True, node3.isLeader())
        self.assertEqual(False, node2.isLeader())
        self.assertEqual(False, node1.isLeader())
        self.assertEqual(int, node2.whoseLeader) # Default value of int

    def test_LowestBecomesLeader_When_AllOthersDead(self):
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)
        node3 = OriginalBullyProcess(3)
        node4 = OriginalBullyProcess(4)
        node5 = OriginalBullyProcess(5)

        nodeArray = [node1, node2, node3, node4, node5]
        nodesToKill = [node2, node3, node4, node5]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)
        
        for nodeKill in nodesToKill:
            nodeKill.killNode()

        node1.startElection()
        
        # Assert that lowest is leader
        self.assertEqual(False, node5.isLeader())
        self.assertEqual(False, node4.isLeader())
        self.assertEqual(False, node3.isLeader())
        self.assertEqual(False, node2.isLeader())
        self.assertEqual(True, node1.isLeader())

    def test_HighestElected_When_LowestKilledAndStarted(self):
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)
        node3 = OriginalBullyProcess(3)
        node4 = OriginalBullyProcess(4)
        node5 = OriginalBullyProcess(5)

        nodeArray = [node1, node2, node3, node4, node5]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)
        
        node1.killNode()
        node2.startElection()
        
        # Assert that highest is leader after node kill
        self.assertEqual(True, node5.isLeader())
        self.assertEqual(False, node4.isLeader())
        self.assertEqual(False, node3.isLeader())
        self.assertEqual(False, node2.isLeader())
        self.assertEqual(False, node1.isLeader())

        # Assert that highest is leader after node start
        node1.startNode()
        self.assertEqual(True, node5.isLeader())
        self.assertEqual(False, node4.isLeader())
        self.assertEqual(False, node3.isLeader())
        self.assertEqual(False, node2.isLeader())
        self.assertEqual(False, node1.isLeader())
        self.assertEqual(node5.id, node1.whoseLeader) # Assert that node1 gets correct leader


if __name__ == '__main__':
    unittest.main()
