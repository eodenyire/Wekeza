export default function AboutPage() {
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <h1 className="text-4xl font-bold mb-8">About Wekeza Bank</h1>
      <div className="prose max-w-none">
        <p className="text-lg text-gray-700 mb-6">
          Wekeza Bank is a modern financial institution committed to providing innovative banking solutions
          to individuals, businesses, and enterprises across the region.
        </p>
        <h2 className="text-2xl font-bold mb-4">Our Mission</h2>
        <p className="text-gray-700 mb-6">
          To empower our customers with accessible, secure, and innovative banking services that help them
          achieve their financial goals.
        </p>
        <h2 className="text-2xl font-bold mb-4">Our Values</h2>
        <ul className="list-disc list-inside space-y-2 text-gray-700">
          <li>Customer First</li>
          <li>Innovation</li>
          <li>Integrity</li>
          <li>Excellence</li>
        </ul>
      </div>
    </div>
  );
}
