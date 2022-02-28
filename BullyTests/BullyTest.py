import unittest
from OriginalBullyProcess import OriginalBullyProcess


class MyTestCase(unittest.TestCase):
    def test_4IsElected(self):
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)
        node3 = OriginalBullyProcess(3)
        node4 = OriginalBullyProcess(4)

        nodeArray = [node1, node2, node3, node4]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        node1.startElection()

        self.assertEqual(True, node4.isLeader())  # add assertion here
        self.assertEqual(False, node3.isLeader())  # add assertion here
        self.assertEqual(False, node2.isLeader())  # add assertion here
        self.assertEqual(False, node1.isLeader())  # add assertion here

    def test_node2Killed_1isElected(self):
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)

        nodeArray = [node1, node2]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        node2.killNode()
        node1.startElection()

        print(node2.isLeader())
        print(node1.isLeader())

        self.assertEqual(False, node2.isLeader())  # add assertion here
        self.assertEqual(True, node1.isLeader())  # add assertion here

    def test_node2KilledAndStarted_2isElected(self):
        node1 = OriginalBullyProcess(1)
        node2 = OriginalBullyProcess(2)

        nodeArray = [node1, node2]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        node2.killNode()
        node1.startElection()
        node2.startNode()

        print(node2.isLeader())
        print(node1.isLeader())

        self.assertEqual(True, node2.isLeader())  # add assertion here
        self.assertEqual(False, node1.isLeader())  # add assertion here



if __name__ == '__main__':
    unittest.main()
